namespace Hajir.Crm.Products
{
    public class CabinetLocation
    {
        public int Row { get; private set; }
        public int Column { get; private set; } = 0;
        public bool Filled { get; set; }
        public int Qty => Filled ? 1 : 0;

        public string Pic => Filled ? "X" : ".";

        public CabinetLocation(int row, int column, bool filled)
        {
            Row = row;
            Column = column;
            Filled = filled;
        }
        public void Clear() => Filled = false;
        public bool Fill(bool forced = false)
        {
            if (!Filled || forced)
            {
                Filled = true;
                return true;
            }
            return false;
        }

    }
}

