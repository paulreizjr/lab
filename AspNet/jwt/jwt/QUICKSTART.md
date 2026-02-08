# 🚀 Quick Start Guide - JWT Authentication

## Run the Application

```bash
dotnet run
```

The API will start at `https://localhost:5001` (or check the console output for the URL)

---

## Test in 3 Simple Steps

### Step 1: Login to Get Token

**Using Swagger UI** (Easiest):
1. Open browser: `https://localhost:5001/swagger`
2. Expand `POST /api/auth/login`
3. Click "Try it out"
4. Use this body:
   ```json
   {
     "username": "john.doe",
     "password": "password123"
   }
   ```
5. Click "Execute"
6. **Copy the token** from the response

### Step 2: Authorize in Swagger

1. Click the **"Authorize"** button at the top
2. Enter: `Bearer YOUR_TOKEN_HERE` (replace with actual token)
3. Click "Authorize"
4. Close the dialog

### Step 3: Test Protected Endpoints

Now you can call any endpoint:
- `GET /api/securedata/protected` - Basic authentication
- `GET /api/securedata/me` - Get current user
- `GET /api/securedata/admin` - Admin only (will fail with mock user)
- `GET /api/securedata/management` - Manager/Admin access

---

## Example cURL Commands

### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john.doe","password":"password123"}' \
  -k
```

### Use Token
```bash
curl -X GET https://localhost:5001/api/securedata/protected \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -k
```

**Note:** `-k` flag skips SSL certificate verification for local development

---

## Understanding the Flow

```
1. POST /api/auth/login (username + password)
   └─> Returns JWT token

2. Store token securely

3. Include token in requests:
   Header: Authorization: Bearer {token}

4. Access protected endpoints
   └─> Server validates token automatically
```

---

## Key Points

✅ **Mock Authentication**: Current implementation accepts any username with password `"password123"`

✅ **Token Expiration**: Tokens expire in 60 minutes (configurable in appsettings.json)

✅ **Roles**: Mock tokens include "User" and "Manager" roles

⚠️ **Production**: Replace mock authentication with real database lookup and password hashing

---

## Next Steps

1. **Review Code Comments**: All files have extensive inline documentation
2. **Read README.md**: Comprehensive guide on when to use/not use JWT
3. **Check jwt.http**: Ready-to-use HTTP requests for testing
4. **Implement Real Auth**: Replace mock validation with ASP.NET Core Identity

---

## Troubleshooting

### "401 Unauthorized" Error
- Check token is included: `Authorization: Bearer {token}`
- Verify token hasn't expired (60 min default)
- Ensure "Bearer " prefix is present

### "403 Forbidden" Error
- User is authenticated but lacks required role
- Check endpoint role requirements

### Token Not Working
- Decode at https://jwt.io to inspect claims
- Verify issuer and audience match config
- Check expiration time (exp claim)

---

**Happy Coding! 🎉**
