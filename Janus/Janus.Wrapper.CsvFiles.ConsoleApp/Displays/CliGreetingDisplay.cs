using Janus.Wrapper.Core;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class CliGreetingDisplay : BaseDisplay
{
    private readonly WrapperOptions _wrapperOptions;
    public CliGreetingDisplay(WrapperController wrapperController, WrapperOptions wrapperOptions) : base(wrapperController)
    {
        _wrapperOptions = wrapperOptions;
    }

    protected async override Task<Result> Display()
    {
        System.Console.WriteLine("Welcome to the Janus Wrapper CLI application!");
        System.Console.WriteLine(_janusAscii);
        System.Console.WriteLine($"This is Wrapper {_wrapperOptions.NodeId} listening on port {_wrapperOptions.ListenPort}");

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
