using FunctionalExtensions.Base.Resulting;

namespace Janus.Mediator.ConsoleApp.Displays;
public class UiGreetingDisplay : BaseDisplay
{
    private readonly MediatorOptions _mediatorOptions;
    public UiGreetingDisplay(MediatorManager MediatorManager, MediatorOptions mediatorOptions) : base(MediatorManager)
    {
        _mediatorOptions = mediatorOptions;
    }

    protected async override Task<Result> Display()
    {
        System.Console.WriteLine("Welcome to the Janus Mediator UI application!");
        System.Console.WriteLine(_janusAscii);
        System.Console.WriteLine($"This is Mediator {_mediatorOptions.NodeId} listening on port {_mediatorOptions.ListenPort}");

        return await Task.FromResult(Results.OnSuccess());
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
