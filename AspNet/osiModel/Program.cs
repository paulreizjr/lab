namespace osiModel;

// =====================================================================================
// HTTP(S) DATA TRANSFER - COMPLETE STEP-BY-STEP GUIDE
// =====================================================================================
//
// This document explains what happens during an HTTP(S) data transfer for a request like:
// https://example.com/path
//
// Complete end-to-end flow: client → network → server → back → browser
// Including OSI layer mapping, HTTP versions (1.1/2/3), caching, and failure points.
//
// =====================================================================================

// =====================================================================================
// 📚 FUNDAMENTAL CONCEPTS & TERMINOLOGY
// =====================================================================================

// 🌐 NETWORKING PROTOCOLS:
// • TCP (Transmission Control Protocol) - Provides reliable, ordered delivery of data
// • UDP (User Datagram Protocol) - Faster but unreliable data transmission  
// • TLS (Transport Layer Security) - Cryptographic protocol for secure communication
//   - Ensures confidentiality, integrity, and authenticity
//   - Operates at OSI Presentation layer (Layer 6)
//   - Used for HTTPS, email, messaging
// • SSL (Secure Sockets Layer) - Predecessor to TLS, now obsolete and insecure
// • QUIC - Modern protocol combining transport and security (used in HTTP/3)

// 📋 PROTOCOL USAGE BY TYPE:
// TCP-based: HTTP, HTTPS, FTP, SMTP, POP3, IMAP, SSH, Telnet, WebSockets
// UDP-based: DNS, NTP, DHCP, SNMP, TFTP, RTP, QUIC

// 🔑 KEY NETWORKING TERMS:
// • Port - Like a mailbox for network traffic (e.g., 80=HTTP, 443=HTTPS)
// • RTT (Round-Trip Time) - Time for signal to go client→server→back
// • Hop - Step in data path from source to destination (router to router)
// • Gateway - Local router connecting to wider internet
// • TTL (Time To Live) - How long DNS records can be cached

// =====================================================================================
// 🚀 STEP 1: REQUEST INITIATION (OSI: Application Layer)
// =====================================================================================

// 📝 URL PARSING:
// Browser splits URL into components:
// • Scheme: https
// • Host: example.com  
// • Path: /path

// 💾 CACHE CHECKING (Performance Optimization):
// Browser checks multiple cache layers before making network request:

// 1️⃣ SERVICE WORKER CACHE:
//    • Programmable network proxy running in separate thread
//    • Intercepts requests and serves cached/custom responses
//    • Enables offline support, background sync, push notifications
//    • Registered for specific URL patterns/scopes
//    • Uses Cache API for fine-grained caching control

// 2️⃣ HTTP CACHE:
//    • Stores previous responses to speed up repeated requests
//    • Uses server headers: Cache-Control, ETag, Last-Modified
//    • Server tells browser how long to cache and how to validate

// 3️⃣ HSTS CACHE (Security):
//    • HTTP Strict Transport Security
//    • Forces HTTPS for all requests to domain
//    • Prevents man-in-the-middle attacks and cookie hijacking
//    • Based on Strict-Transport-Security header from server

// 4️⃣ DNS CACHE:
//    • Stores resolved domain names
//    • Reduces latency by avoiding repeated DNS lookups

// 5️⃣ CONNECTION POOL:
//    • Maintains open TCP/TLS connections for reuse
//    • Reduces connection establishment overhead
//    • Uses keep-alive to maintain connections after requests
//    • Multiple requests can use same connection (improves performance)

// ⚡ PERFORMANCE OPTIMIZATIONS:
// • Preconnect - Establish early DNS/TCP/TLS connections before actual request
// • Prefetch - Download resources that might be needed soon

// 🔄 PROTOCOL SELECTION:
// Browser chooses HTTP version (1.1/2/3) based on:
// • Prior knowledge
// • ALPN (Application-Layer Protocol Negotiation) from TLS
// • Reuses existing connection if keep-alive is active

// =====================================================================================
// 🔍 STEP 2: DNS RESOLUTION (OSI: Application → Network)
// =====================================================================================

// 📍 IP ADDRESS LOOKUP:
// If client needs server IP, it resolves 'example.com':
// Check browser cache → OS resolver cache → hosts file → recursive DNS resolver → root/TLD/authoritative servers

// 📋 DNS RECORD TYPES:
// • A (IPv4) - Maps domain to IPv4 address
// • AAAA (IPv6) - Maps domain to IPv6 address  
// • CNAME (alias) - Points to another domain
// • MX (mail) - Mail exchange servers
// • TXT (text) - Text records for verification/configuration

// 🔒 SECURE DNS VARIANTS:
// • DNS over HTTPS (DoH) - DNS queries over HTTPS for privacy
// • DNS over TLS (DoT) - DNS queries over TLS for privacy

// 📊 RESULT: IP address (IPv4/IPv6) + TTL (Time To Live for caching)

// =====================================================================================
// 🔗 STEP 3: LINK LAYER RESOLUTION (OSI: Data Link / Physical)
// =====================================================================================

// 🏠 LOCAL NETWORK MAPPING:
// • ARP (Address Resolution Protocol) - Maps IPv4 addresses to MAC addresses
// • NDP (Neighbor Discovery Protocol) - IPv6 equivalent of ARP
// • Client resolves MAC address of gateway (local router)
// • Creates Ethernet frames for next hop transmission

// =====================================================================================
// 🤝 STEP 4: TCP CONNECTION ESTABLISHMENT (OSI: Transport + Network)
// =====================================================================================

// ❓ CAN WE SKIP TCP?
// • HTTP/1.1 & HTTP/2: NO - Require TCP for reliable, ordered delivery
// • HTTP/3: YES - Uses QUIC over UDP (integrates transport + security)
// • WebSockets, FTP, SMTP, SSH, Telnet: NO - All require TCP
// • DNS: MIXED - Primarily UDP, but uses TCP for large responses

// 🤝 TCP THREE-WAY HANDSHAKE (for HTTP/1.1 & HTTP/2):
// 1. Client → Server: SYN (choose ephemeral source port)
// 2. Server → Client: SYN-ACK  
// 3. Client → Server: ACK

// 📊 ESTABLISHES:
// • Sequence numbers - Track byte order for correct reassembly
// • Receive windows - Flow control (how much data receiver can handle)

// 🖥️ PROCESS HANDLING:
// • Server: OS kernel's TCP/IP stack handles handshake
// • Client: OS kernel's TCP/IP stack handles handshake
// • Ports: Server (80=HTTP, 443=HTTPS), Client (ephemeral port)

// ⚡ OPTIMIZATION: TCP Fast Open (allows data in initial SYN packet)

// =====================================================================================
// 🔐 STEP 5: TLS HANDSHAKE (HTTPS Only - OSI: Transport + Presentation)
// =====================================================================================

// 🔄 TLS NEGOTIATION PROCESS:
// Performed OVER the established TCP connection:

// 1️⃣ ClientHello:
//    • Supported TLS versions (1.2, 1.3)
//    • Cipher suites (encryption + key exchange + hashing algorithms)
//    • SNI (Server Name Indication) - Multiple domains on same IP
//    • ALPN (Application-Layer Protocol Negotiation) - http/1.1 or h2

// 2️⃣ ServerHello:
//    • Chosen protocol/cipher from client options
//    • Server certificate chain (X.509 format)

// 3️⃣ Certificate Validation (Client):
//    • Chain validation - Issued by trusted CA
//    • Signature verification  
//    • Hostname matching
//    • Expiry check
//    • Revocation check (OCSP)

// 4️⃣ Key Exchange:
//    • Modern: ECDHE (Elliptic Curve Diffie-Hellman Ephemeral)
//    • Both sides derive shared secret without sending it over network
//    • Generate session keys for encryption (AES, ChaCha20, etc.)

// 5️⃣ Handshake Completion:
//    • ChangeCipherSpec - Switch to encrypted mode
//    • Finished messages - Prove handshake integrity

// 🖥️ PROCESS HANDLING:
// • Server: Web server (nginx, Apache, IIS) or app server (Node.js, ASP.NET)
// • Client: Browser or HTTP client library (curl, requests)
// • Port: Typically 443 for HTTPS

// ⚡ OPTIMIZATIONS:
// • TLS 1.3 reduces round trips vs TLS 1.2
// • Session resumption with session tickets/IDs
// • 0-RTT for repeated connections

// =====================================================================================
// 📤 STEP 6: HTTP REQUEST TRANSMISSION (OSI: Application)
// =====================================================================================

// 📝 REQUEST FORMAT (HTTP/1.1 Example):
// ```
// GET /path HTTP/1.1
// Host: example.com
// User-Agent: Mozilla/5.0...
// Accept: text/html
// Accept-Encoding: gzip, br
// Connection: keep-alive
// Cookie: session=abc123
// ```

// 🔑 IMPORTANT HEADERS:
// • Host - Virtual hosting (multiple sites on same server)
// • Accept-Encoding - Compression support (gzip, brotli)
// • Cookie - Session/state information
// • Authorization - Authentication credentials
// • If-None-Match/If-Modified-Since - Conditional requests for caching

// 📊 REQUEST VARIATIONS:
// • POST/PUT: Include request body + Content-Length (or chunked transfer)
// • HTTP/2: Binary frames + header compression (HPACK)
// • HTTP/3: QUIC frames with built-in multiplexing

// =====================================================================================
// 🌐 STEP 7: NETWORK TRAVERSAL (OSI: Network / Data Link)
// =====================================================================================

// 🛣️ PACKET ROUTING:
// Packets travel across multiple network components:
// • Routers - Forward packets toward destination
// • NAT (Network Address Translation) - Maps private to public IPs
// • Firewalls - Filter traffic based on rules
// • Load balancers - Distribute requests across servers
// • CDN edges - Cache content closer to users
// • Proxies - Intermediate request handlers

// =====================================================================================
// 🏢 STEP 8: SERVER REQUEST HANDLING (OSI: Application)
// =====================================================================================

// 🔄 SERVER PROCESSING FLOW:
// 1️⃣ TLS Termination:
//    • May terminate at load balancer (TLS offload)
//    • Or passed through to upstream servers

// 2️⃣ Web Server Layer:
//    • nginx, Apache, IIS accepts request
//    • Static content served directly
//    • Dynamic requests forwarded to application server

// 3️⃣ Application Server:
//    • Node.js, Django, ASP.NET, etc.
//    • Business logic processing
//    • Database queries, cache lookups
//    • External service calls

// 4️⃣ Middleware Processing:
//    • Authentication/authorization
//    • Rate limiting  
//    • Logging and tracing
//    • Request validation

// =====================================================================================
// 📤 STEP 9: HTTP RESPONSE GENERATION (OSI: Application)
// =====================================================================================

// 📊 RESPONSE COMPONENTS:
// Status Line: HTTP/1.1 200 OK

// 🔑 RESPONSE HEADERS:
// • Content-Type - MIME type of response body
// • Content-Length - Exact size of response body
// • Transfer-Encoding: chunked - Stream without knowing total size
// • Cache-Control - Caching directives (public, max-age, no-cache)
// • ETag - Version identifier for caching
// • Set-Cookie - Send cookies to client
// • Content-Encoding - Compression used (gzip, brotli)
// • Strict-Transport-Security - Force HTTPS (HSTS)

// 🍪 COOKIES EXPLAINED:
// • Small data pieces stored on client side
// • Used for: session management, personalization, tracking
// • Server sends via Set-Cookie header
// • Client returns via Cookie header
// • Security flags:
//   - Secure: Only sent over HTTPS
//   - HttpOnly: Not accessible via JavaScript
//   - SameSite: Controls cross-site sending

// 📦 RESPONSE BODY:
// • Compressed if Content-Encoding specified
// • Chunked if Transfer-Encoding: chunked used
// • Allows streaming of large responses

// =====================================================================================
// 🔄 STEP 10: RESPONSE NETWORK TRAVERSAL (OSI: Network + Transport)
// =====================================================================================

// 🛣️ RETURN PATH:
// • Response travels back over same network path
// • May use CDN edge caches for static content
// • TCP ensures reliable delivery:
//   - Segmentation for large responses
//   - ACKnowledgments for received packets
//   - Flow control via windowing
//   - Retransmission on packet loss

// 🚀 HTTP VERSION DIFFERENCES:
// • HTTP/1.1: One request/response per connection (or sequential)
// • HTTP/2: Multiple requests/responses multiplexed on single TCP connection
// • HTTP/3: QUIC over UDP, streams avoid head-of-line blocking

// =====================================================================================
// 📥 STEP 11: CLIENT RESPONSE PROCESSING (OSI: Application)
// =====================================================================================

// 🔓 DECRYPTION & PARSING:
// 1️⃣ TLS decrypts response and provides plaintext HTTP to browser

// 2️⃣ Header Processing:
//    • Cache-Control/ETag - Store response in cache if appropriate
//    • Redirects (3xx) - Issue new request to Location header
//    • Set-Cookie - Store cookies respecting security flags
//    • Content-Encoding - Decompress body (gzip/brotli)

// 3️⃣ Content Processing:
//    • HTML: Parse and build DOM
//    • Discover sub-resources (CSS, JS, images)
//    • Issue additional requests (may reuse TCP/TLS connection)
//    • JavaScript XHR/fetch requests trigger same flow

// =====================================================================================
// 🎨 STEP 12: BROWSER RENDERING & RESOURCE LOADING
// =====================================================================================

// 🏗️ RENDER PIPELINE:
// • Construct render tree (DOM + CSSOM)
// • Layout calculation (positioning)
// • Paint (visual rendering)

// ⚡ PERFORMANCE FACTORS:
// • Blocking resources (CSS/JS) can delay rendering
// • Browser attempts parallelism when possible
// • HTTP/2 multiplexing improves parallel resource loading
// • HTTP/3 further improves with independent streams

// =====================================================================================
// 🔌 STEP 13: CONNECTION LIFECYCLE (OSI: Transport)
// =====================================================================================

// 🔄 CONNECTION REUSE:
// • Persistent connections (Connection: keep-alive)
// • Multiple requests over single TCP/TLS connection
// • Reduces handshake overhead significantly

// ⏰ CONNECTION TERMINATION:
// • Idle timeout closes unused connections
// • Clean close: TCP FIN/ACK exchange
// • TLS session resumption for faster future handshakes

// =====================================================================================
// 💾 STEP 14: CACHING & CONDITIONAL REQUESTS
// =====================================================================================

// 🎯 CACHE VALIDATION:
// Browser and intermediaries use caching headers:
// • Cache-Control - Caching behavior directives
// • Expires - Absolute expiration time
// • ETag - Entity tag for content version
// • Last-Modified - When resource was last changed

// 🔄 CONDITIONAL REQUEST EXAMPLE:
// Client sends: If-None-Match: "abc123"
// Server responds: 304 Not Modified (if unchanged)
// → Client reuses cached response body

// =====================================================================================
// 🏗️ STEP 15: INFRASTRUCTURE COMPONENTS
// =====================================================================================

// 🔄 PROXY TYPES:
// • Forward proxy - Client-side (corporate networks, privacy)
// • Reverse proxy/Load balancer - Server-side (distribute load)

// 🌐 CDN (Content Delivery Network):
// • Caches static resources at edge locations
// • Reduces latency by serving from nearby servers
// • May handle TLS termination and caching rules

// 🛡️ WAF (Web Application Firewall):
// • Inspects HTTP traffic for malicious patterns
// • Blocks attacks (SQL injection, XSS, etc.)

// 🔧 HEADER MODIFICATIONS:
// Middleboxes may add/modify headers:
// • X-Forwarded-For - Original client IP
// • X-Real-IP - Client IP through proxy
// • Via - Proxy chain information

// =====================================================================================
// 📊 STEP 16: OBSERVABILITY & MONITORING
// =====================================================================================

// 🔍 TRACING HEADERS:
// • X-Request-ID - Correlate logs across services
// • traceparent - Distributed tracing standard
// • Custom correlation IDs

// 📈 PERFORMANCE METRICS:
// • DNS lookup time
// • TCP connect time  
// • TLS handshake time
// • TTFB (Time To First Byte)
// • Content download time
// • Total page load time

// =====================================================================================
// ❌ STEP 17: ERROR HANDLING & FAILURE MODES
// =====================================================================================

// 🚫 COMMON FAILURES:
// • DNS failure - Cannot resolve hostname
// • TCP timeout/refused - Server down or firewall blocking
// • TLS handshake failure - Certificate issues, cipher mismatches
// • HTTP 4xx - Client errors (404 Not Found, 401 Unauthorized)
// • HTTP 5xx - Server errors (500 Internal Error, 503 Service Unavailable)
// • Partial downloads - Connection reset or aborted

// 🔧 DEBUGGING APPROACH:
// Check each layer: DNS → TCP → TLS → HTTP → Application

// =====================================================================================
// 🚀 STEP 18: HTTP VERSION VARIATIONS
// =====================================================================================

// 📈 HTTP/1.1 (Traditional):
// • Text-based protocol
// • One request per connection (or sequential with keep-alive)
// • Header redundancy
// • Head-of-line blocking

// ⚡ HTTP/2 (Modern):
// • Binary framing protocol
// • Multiplexing - Multiple streams over single TCP connection
// • Header compression (HPACK)
// • Server push capability (rarely used)
// • Still uses TCP (head-of-line blocking at transport layer)

// 🚀 HTTP/3 (Latest):
// • Built on QUIC over UDP
// • Integrates transport and security layers
// • Eliminates head-of-line blocking
// • Faster connection establishment
// • Better packet loss recovery
// • 0-RTT connection resumption

// =====================================================================================
// ⏱️ STEP 19: PERFORMANCE ANALYSIS - RTT COSTS
// =====================================================================================

// 🐌 TYPICAL FIRST REQUEST LATENCY:
// Without optimizations, first meaningful byte requires:
// 1. DNS lookup RTT (if not cached)
// 2. TCP handshake RTT  
// 3. TLS handshake RTT (unless resumed/TLS 1.3)
// 4. HTTP request → first byte RTT
// = MINIMUM 4 RTTs for first request

// ⚡ PERFORMANCE OPTIMIZATIONS:
// • Connection reuse (keep-alive)
// • HTTP/2 multiplexing
// • HTTP/3 integrated transport
// • TLS 1.3 faster handshake
// • CDNs for geographic proximity
// • Preconnect/prefetch hints
// • Compression (gzip, brotli)
// • Caching at all levels

// =====================================================================================
// 📋 QUICK REFERENCE CHEAT SHEET
// =====================================================================================

// 🔄 COMPLETE FLOW:
// DNS → TCP handshake → TLS handshake → HTTP request → 
// server processing → HTTP response → browser parse/render → 
// connection reuse/close

// 🔑 CRITICAL HEADERS:
// Request: Host, Connection, Accept-Encoding, Cookie, Authorization
// Response: Content-Type, Cache-Control, Set-Cookie, ETag, Content-Encoding

// ⚡ SPEED BOOSTERS:
// • Keep-alive connections
// • HTTP/2 or HTTP/3
// • TLS 1.3
// • CDN usage
// • Compression (brotli > gzip)
// • Aggressive caching
// • Connection preconnect
// • Resource prefetch

// =====================================================================================
// 🏗️ ASP.NET CORE DEPLOYMENT NOTES
// =====================================================================================

// 🔌 MULTIPLE PROJECTS ON SAME SERVER:
// Different ports required for each project:
// • Project 1: http://localhost:5000/api1
// • Project 2: http://localhost:5001/api2
// 
// 🔄 SINGLE PORT SOLUTION (Reverse Proxy):
// Use nginx/IIS to route based on URL path:
// • nginx listens on port 80/443
// • Routes /api1/* to localhost:5000  
// • Routes /api2/* to localhost:5001
// 
// Benefits: Single entry point, SSL termination, load balancing

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}