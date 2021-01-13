using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
namespace ThreeDimClock
{
	class Viewer3d
	{
		public char[,,] space;
		public int Center{get {return center;}}
		private char[,,] rotatedSpace;
		private int center = 0;
		private StringBuilder sb = new StringBuilder();
		
		public Viewer3d(int worldSize)
		{
			center = worldSize/2;			
			space = new char[worldSize,worldSize,worldSize];
			rotatedSpace = new char[worldSize,worldSize,worldSize];
			Clear(space);
		}	
		public void Start()
		{
			System.Drawing.Point scrPt = Cursor.Position;
			new Thread(()=>
			{
				DateTime n = DateTime.Now;
				int delay = 30;
				double sensitivity = 0.07d;
				while(true)
				{
					n = DateTime.Now;
					n = n.AddMilliseconds(delay);
					Spin((Cursor.Position.Y - scrPt.Y) * sensitivity, (Cursor.Position.X - scrPt.X) * sensitivity);
					while(n > DateTime.Now){Thread.Sleep(10);}
					Print();
				}
			}).Start();
			new Thread(()=>
			{
				while(true)
					if(Console.ReadKey(true).Key == ConsoleKey.Spacebar)
						 scrPt = Cursor.Position;
			}).Start();
		}		
		void Print()
		{
			char a = ' ';
			for(int i = 0; i < rotatedSpace.GetLength(0); i++)
			{
				for(int j = 0; j < rotatedSpace.GetLength(2); j++)
				{
					a = ' ';
					for(int k = 0; k < rotatedSpace.GetLength(1); k++)
						if(rotatedSpace[i,k,j] != ' ')
						{
							a = rotatedSpace[i,k,j]; break;
						}
					sb.Append(a);
				}
				sb.Append('\n');
			}
			Console.SetCursorPosition(0,0);
			Console.WriteLine(sb.ToString());
			Clear(rotatedSpace);
		}
		void Clear(char[,,] b)
		{
			sb.Clear();
			for(int i = 0; i < b.GetLength(0); i++)
				for(int j = 0; j< b.GetLength(1); j++)
					for(int k = 0; k < b.GetLength(2); k++)
						b[i,j,k] = ' ';
		}		
		void Spin(double rot_x, double rot_y)
		{
			double x1,y1,y2,z1;
			double degreeX = rot_x * (Math.PI / 180d);
			double degreeY = rot_y * (Math.PI / 180d);
			double sinX = Math.Sin(degreeX);
			double sinY = Math.Sin(-degreeY);
			double cosX = Math.Cos(degreeX);
			double cosY = Math.Cos(-degreeY);
			
			for(int i = -center; i < center; i++)//y
				for(int j = -center; j< center; j++)//z
					for(int k = -center; k < center; k++)//x
					{
						y1 = j * cosY + k * sinY;
						z1 = j * -sinY + k * cosY;		
						x1 = i * cosX + y1 * sinX;
						y2 = i * -sinX + y1 * cosX;
						if(x1 >= -center && y2 >= -center && z1 >= -center && x1 < center && y2 < center && z1 < center)
						rotatedSpace[i + center,j + center,k + center] = space[(int)x1 + center,(int)y2 + center,(int)z1 + center];
					}		
		}
	}
}
	 
	 
