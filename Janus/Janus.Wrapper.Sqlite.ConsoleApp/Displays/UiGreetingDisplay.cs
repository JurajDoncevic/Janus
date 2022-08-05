using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.ConsoleApp.Displays;
public class UiGreetingDisplay : BaseDisplay
{
    private readonly WrapperOptions _mediatorOptions;
    public UiGreetingDisplay(WrapperController mediatorController, WrapperOptions mediatorOptions) : base(mediatorController)
    {
        _mediatorOptions = mediatorOptions;
    }

    protected async override Task<Result> Display()
    {
        System.Console.WriteLine("Welcome to the Janus Wrapper UI application!");
        System.Console.WriteLine(_janusAscii);
        System.Console.WriteLine($"This is Wrapper {_mediatorOptions.NodeId} listening on port {_mediatorOptions.ListenPort}");

        return await Task.FromResult(Result.OnSuccess());
    }

    private readonly string _janusAscii = @"
                                    __  __  _
             _.-._ _.-'-._  _   _.-  .-' -'   '-._
          .-'-- ._'-._'-. '-.'-._..-'__.-'  _.-'   '-.
       .-' '-._ '-._    '-._ '-. _    _.--'     _/   ''-.
    .-''''-._.. \_      _                    _.-'  _.-'  _\
  .'        \_          \_      '-._/_.-'  _.-   /__.- '  \
 |       _.'        '-._    '-._  _ _.-' _/      _.-'   _.'(
 \'-._ .'         )               _/     __/       _.-'/ _.'
  '._  \_     _.'  '-._/  -._  _.'      _/     _/ / __.-'(
      \'-._'-.)_         ._       _.-'_/    _(_ _.'      )
      (    '-.__'-.  '-.   '--.__.          _.'          |
      /         ('-.      '-._      __/   _.'    .;:::=. |
     (   .-===.  ('-.          '-._  _/  _/     /  .-.   )
      ' ' .-.  \  ''-.   '-._           _.'       '-._) (
       ) (_.-'  :   '-.         _.'     _/'      '--=.   '.
     .'  '---= '     '-. \_       _.-'    '          )     '.
    ;       .'       '-.    ._          _.)         /        \
   '      (          '-.          _/     _'        /    _.--  )
  /        \         '-.    '-.  )'-._  '.-.     .'   .' __.-'
 (  -._   '.\        '-. '-._  -.'-._   _'-.    (   _.-'.-'
  '-.__'---' \       '-.   . '_'('        _.\   _.-'  ___)
     '-.'-.._ \      '-.   '-.' ) _.-'   _.-'''./'._ '._)
     '-.'-.'-. )    '-.     '( ) )   _.-'   .-''-.__'-.-'
     ('__\ '-.    '-.     '-.))_\ _.'   _.-'  _.-'_.-'.-'
      (_.''-._'-.'-.'-.''-._'  '-.  _.-'  _.-'  _.'-.'_.'
       '-.'-.'-.'-.'-._'-._'     '-. _.-'   _.-'  ).-'_.'
      '-.'-.'-_.'-.'-.'-._'         '-._ _.'_.).-' _.-'
     '-.'-.'-.'-.'-.'-._'              '-.__.-'_.--'
    '-._'-.'-.'-.'-.'-.  
     '-.'-.'-.'-.'-.'
       '-.'-.'-.'-'
    ";

    public override string Title => "GREETINGS";

    protected override void PostDisplay()
    {

    }

    protected override void PreDisplay()
    {

    }
}
