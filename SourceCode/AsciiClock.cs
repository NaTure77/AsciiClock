using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreeDimClock
{
	class XY<T>
	{
		public T x, y;
		public XY(T x, T y)
		{
			this.x = x; this.y = y;
		}
	}
	class AsciiClock
	{
		char[,] face;
		char[,] display;
		int secondHandLength;
		int minuteHandLength;
		int hourHandLength;
		XY<int> segPos;
		XY<int> cronoPos;
		int center;
		int radius;
		
		public AsciiClock(int radius)
		{	
			this.radius = radius;
			
			int width = radius * 2 + 1;
			face = new char[width, width];
			display = new char[width, width];
			
			center = radius + 1;
			secondHandLength = (int)(radius * 0.9f);
			minuteHandLength = (int)(radius * 0.7f);
			hourHandLength = (int)(radius * 0.5f);
			
			//추후수정
			segPos = new XY<int>(radius/2,radius/2+5);		
			cronoPos = new XY<int>(radius + radius / 6 + 1, radius + radius / 6 + 1);

			Make_FaceDesign();
		}
		public char[,] GetDisplay(){ return display; }
		
		public void Make_FaceDesign()
		{
			//Generating clock frame
			for(int i = 0; i < face.GetLength(0); i++)
				for(int j = 0; j < face.GetLength(1); j++)
				{
					if(Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) > Math.Pow(radius - 1, 2) &&
					   Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) < Math.Pow(radius, 2))
					   face[i,j] = '#';
					  
					else if(Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) < Math.Pow(radius - 5, 2)&&
					        Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) > Math.Pow(radius - 7, 2))
							face[i,j] = '=';
					else face[i,j] = ' ';
					
					if(Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) < Math.Pow(3,2))
					{
						if(Math.Pow(i - radius-1,2) + Math.Pow(j - radius - 1, 2) < Math.Pow(2,2))
							face[i,j] = ' ';
						else face[i,j] = 'O';
					}
				}
			//additional decoration
			for(int i = 0; i < 12; i++)
				for(int j = 0; j < 5; j++)
					for(int k = 0; k < 6; k++)
						Rotate(radius - 1 + j, k + 2, center, center, '-', -i * 30, face);
		}
		public void Start(Action PrintMethod)
		{
			new Thread(()=>
			{
				DateTime n = DateTime.Now;
				int second = DateTime.Now.Second;
				while(true)
				{
					n = DateTime.Now;
					n = n.AddMilliseconds(100);				
					while(n > DateTime.Now){Thread.Sleep(30);}
					Draw(n);
					PrintMethod();
				}
			}).Start();
		}
		
		int [] seg = {0x1110111, 0x0100100, 0x1011101, 0x1101101, 0x0101110,
					  0x1101011, 0x1111011, 0x0100111, 0x1111111, 0x1101111}; // 7Segment 0~9 info
		//추후수정
		void DrawSegment(int x, int y, int number, int size, char shape)
		{
			int[] numbers = {number / 10, number % 10};// place value
			for(int n = 0; n < 2; n++)
			{
				if((seg[numbers[n]] & 0x0000001) != 0)for(int i = 0; i < size; i++) display[x+i+1,y] = shape;
				if((seg[numbers[n]] & 0x0000010) != 0)for(int i = 0; i < size; i++) display[x,y+i+1] = shape;
				if((seg[numbers[n]] & 0x0000100) != 0)for(int i = 0; i < size; i++) display[x+size+1,y+i+1] = shape;
				if((seg[numbers[n]] & 0x0001000) != 0)for(int i = 0; i < size; i++) display[x+i+1,y+size+1] = shape;
				if((seg[numbers[n]] & 0x0010000) != 0)for(int i = 0; i < size; i++) display[x,y+i+size+1+1] = shape;
				if((seg[numbers[n]] & 0x0100000) != 0)for(int i = 0; i < size; i++) display[x+size+1,y+i+size+1+1] = shape;
				if((seg[numbers[n]] & 0x1000000) != 0)for(int i = 0; i < size; i++) display[x+i+1,y+size+1+size+1] = shape;
				x += size + 3;
			}
		}
		void DrawHand(int length, double degree, char shape)
		{
			for(int i = center - length; i < center - 2; i++)
				Rotate(center, i, center, center, shape ,degree,display);
		}
		
		//추후수정
		public void Draw(DateTime n)
		{
			for(int i = 0; i < face.GetLength(0); i++)
				for(int j = 0; j < face.GetLength(1); j++)
					display[i,j] = face[i,j];
				
			DrawSegment(segPos.x,segPos.y,n.Hour,5,'*');
			DrawSegment(segPos.x + 20,segPos.y,n.Minute,5,'*');
			DrawSegment(segPos.x + 40,segPos.y,n.Second,5,'*');
			DrawSegment(cronoPos.x - 10,cronoPos.y + 4,n.Month,2,'*');
			DrawSegment(cronoPos.x,cronoPos.y + 4,n.Day,2,'*');
			DrawSegment(cronoPos.x - 24,cronoPos.y + 4,n.Year % 2000,2,'*');
			
			double totalSeconds = n.Hour * 3600 + n.Minute * 60 + n.Second + n.Millisecond/1000d;
			DrawHand(hourHandLength,-totalSeconds/120,'H');
			DrawHand(minuteHandLength,-totalSeconds/10,'M');
			DrawHand(secondHandLength,-totalSeconds*6,'S');
			
		}

		void Rotate(int fromX, int fromY, int pivotX, int pivotY, char shape, double degree, char[,] To)
		{
			double d = degree * (Math.PI / 180d);
			double cos = Math.Cos(d), sin = Math.Sin(d);
			To[(int)Math.Round((fromX - pivotX ) * cos + (fromY - pivotY) *  sin) + pivotX,
			   (int)Math.Round((fromY - pivotY ) * cos + (fromX - pivotX) * -sin) + pivotY]
			= shape;
		}
	}
}