public class PChooseManager {
    public int ChosenAnswer;

    public PChooseManager() {
        ChosenAnswer = -1;
    }

    public int Ask(PPlayer Player, string Title, string[] Options) {
        ChosenAnswer = -1;
        PNetworkManager.NetworkServer.TellClient(Player, new PAskOrder(Title, Options.Length, Options));
        PThread.WaitUntil(() => ChosenAnswer >= 0);
        return ChosenAnswer;
    }
}