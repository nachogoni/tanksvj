public class Layers
{
       public const int Default            = (1 << 0);
       public const int TransparentFX      = (1 << 1);
       public const int IgnoreRaycast      = (1 << 2);

    // ...
       public const int Floor     = (1 << 20);
       public const int Walls     = (1 << 21);
       public const int Props     = (1 << 22);
	   public const int Doors     = (1 << 23);
	
	   public const int FloorNum     = 20;
       public const int WallsNum     = 21;
       public const int PropsNum     = 22;
	   public const int DoorsNum     = 23;
	
       
       public static int GetAllExcept(int layer)
       {
            return (0xFFFF & (~layer));
       }

       //public static int GetAllExceptPlayer()
       //{
       //     return (0xFFFF & (~Player));
       //}
       
       //public static int GetAllExceptPlayers()
       //{
       //     return (0xFFFF & (~(Player | Ghosts)));
       //}
}