namespace UTGame
{
    public class UTBattlePauseNode : _AUIQueueBaseNode
    {
        public override EUIQueueNodeType eNodeType { get {return EUIQueueNodeType.E_ADD_NODE_BATTLE_PAUSE;} }
        public override EUIQueueNode eQueueNode
        {
            get { return EUIQueueNode.ADD; }
        }
        public override string monoGoName
        {
            get { return "battle_pause"; }
        }
    }
}