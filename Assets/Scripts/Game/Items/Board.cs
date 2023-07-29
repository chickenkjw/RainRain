namespace Game.Items
{
    public class Board : Item
    {
        public Board() {
            type = ItemType.Board;
            carryLimit = 10;
        }
    }
}