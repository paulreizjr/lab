using System.Text.Json;

namespace PatternTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Before C# 13
            char escapeChar1 = '\u001B';
            char escapeChar2 = '\x1B';

            // In C# 13
            char escapeChar = '\e';

            // Using the escape character in a string for terminal control (e.g., changing text color)
            string coloredText = "\e[31mRed Text\e[0m";
            Console.WriteLine(coloredText);

            Mediator mediator = new();
            Request request = new() { sRequest = "Initial Request" };
            Response response = mediator.ProcessRequest(request);
        }
    }

    internal interface IStepHandler<TParameter, TResult>
    {
        TResult? Result { get; set; }
        void HandleStep(TParameter parameter);
    }

    internal abstract class BaseStepHandler<TParameter, TResult> : IStepHandler<TParameter, TResult>
    {
        public TResult? Result { get; set; }
        protected IMediator _mediator { get; set; }
        public BaseStepHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public abstract void HandleStep(TParameter parameter);
    }

    internal interface IMediator
    {
        public bool _commitTransaction { get; set; }
        void Notify<TParameter, TResult>(IStepHandler<TParameter, TResult> stepHandler, bool stepFailed = false);
    }

    internal abstract class MediatorBase<TRequest, TResponse> : IMediator
    {
        public bool _commitTransaction { get; set; }
        public abstract TResponse ProcessRequest(TRequest request);
        public abstract void Notify<TParameter, TResult>(IStepHandler<TParameter, TResult> stepHandler, bool stepFailed = false);
        protected void LogStep<TParameter, TResult>(IStepHandler<TParameter, TResult> stepHandler, bool stepFailed = false)
        {
            Type stepType = stepHandler.GetType();
            Console.WriteLine($"Step Handler: {stepType.Name}");
            Console.WriteLine($"Step Failed: {stepFailed}");
            Console.WriteLine($"Result: {JsonSerializer.Serialize(stepHandler.Result)}");
            Console.WriteLine("");
        }
    }

    internal class Mediator : MediatorBase<Request, Response>
    {
        private Response? _response;
        public override Response ProcessRequest(Request request)
        {
            _commitTransaction = true;
            GetComplexType1StepHandler getComplexType1StepHandler = new(this);
            getComplexType1StepHandler.HandleStep(request);
            if (_response is null) return new Response() { ResponseCode = -1 };
            return _response!;
        }
        public override void Notify<TParameter, TResult>(IStepHandler<TParameter, TResult> stepHandler, bool stepFailed = false)
        {
            LogStep(stepHandler, stepFailed);
            if (stepFailed == true)
            {
                _commitTransaction = false;
                return;
            }
            switch (stepHandler)
            {
                case GetComplexType1StepHandler complexType1:
                    GetComplexType2StepHandler getComplexType2 = new(this);
                    getComplexType2.HandleStep(complexType1.Result!);
                    break;
                case GetComplexType2StepHandler complexType2:
                    BuildComplexType3StepHandler buildComplexType3h = new(this);
                    buildComplexType3h.HandleStep(complexType2.Result!);
                    break;
                case BuildComplexType3StepHandler buildComplexType3:
                    BuildResponseStepHandler buildResponseh = new(this);
                    buildResponseh.HandleStep(buildComplexType3.Result!);
                    break;
                case BuildResponseStepHandler buildResponse:
                    _response = buildResponse.Result!;
                    break;
            }
        }
    }

    internal class GetComplexType1StepHandler(IMediator mediator) : BaseStepHandler<Request, ComplexType1>(mediator)
    {
        public override void HandleStep(Request parameter)
        {
            Result = new() { sCt1 = parameter.sRequest, iCt1 = 1 };
            _mediator.Notify<Request, ComplexType1>(this);
            if (_mediator._commitTransaction)
                Console.WriteLine("Transaction Committed.");
            else
                Console.WriteLine("Transaction Rolled Back.");
        }
    }

    internal class GetComplexType2StepHandler(IMediator mediator) : BaseStepHandler<ComplexType1, (ComplexType1, ComplexType2)>(mediator)
    {
        public override void HandleStep(ComplexType1 parameter)
        {
            Result = (parameter, new() { sCt2 = parameter.sCt1, iCt2 = 2 });
            _mediator.Notify<ComplexType1, (ComplexType1, ComplexType2)>(this);
        }
    }

    internal class BuildComplexType3StepHandler(IMediator mediator) : BaseStepHandler<(ComplexType1, ComplexType2), ComplexType3>(mediator)
    {
        public override void HandleStep((ComplexType1, ComplexType2) parameter)
        {
            Result = new() { sCt3 = parameter.Item1.sCt1, iCt3 = parameter.Item2.iCt2 };
            _mediator.Notify<(ComplexType1, ComplexType2), ComplexType3>(this);
        }
    }

    internal class BuildResponseStepHandler(IMediator mediator) : BaseStepHandler<ComplexType3, Response>(mediator)
    {
        public override void HandleStep(ComplexType3 parameter)
        {
            Result = new() { Ct3Response = parameter };
            _mediator.Notify<ComplexType3, Response>(this, true);
        }
    }

    internal class ComplexType1
    {
        public string? sCt1 { get; set; }
        public int iCt1 { get; set; }
    }

    internal class ComplexType2
    {
        public string? sCt2 { get; set; }
        public int iCt2 { get; set; }
    }

    internal class ComplexType3
    {
        public string? sCt3 { get; set; }
        public int iCt3 { get; set; }
    }

    internal class Request
    {
        public string? sRequest { get; set; }
    }

    internal class Response
    {
        public int ResponseCode { get; set; }
        public ComplexType3? Ct3Response { get; set; }
    }
}
