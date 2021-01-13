using System;
using System.Text;
namespace ThreeDimClock
{
	class MainApp
	{
		public static void Main(String[] args)
		{
			int size = 120;
			
			//Setting Console Size & Pixel(font) Size
			Console.CursorVisible = false;
			ConsoleHelper.SetCurrentFont("Terminal", 6);
			ConsoleHelper.SetWindowSize(size + 4, size + 4);
			Console.Title = "Ascii Clock 3D";
			/***************************3D version**************************/
			//Generating Viewer
			Viewer3d viewer = new Viewer3d(size);
			//DrawCube(viewer.space);
			viewer.Start();
			
			//Generating Clock
			AsciiClock clock = new AsciiClock(50);			
			char[,] clockDisplay = clock.GetDisplay();
			
			//Making Print method for clock
			Action PrintClockMethod = ()=>
			{
				int gapX = viewer.Center - clockDisplay.GetLength(0)/2 - 1;
				int gapY = viewer.Center - clockDisplay.GetLength(1)/2 - 1;
				for(int i = 0; i < clockDisplay.GetLength(0); i++)
					for(int j = 0; j < clockDisplay.GetLength(1); j++)
						viewer.space[j + gapY,viewer.Center,i + gapX] = clockDisplay[i,j];
			};
			clock.Start(PrintClockMethod);
			/***************************************************************/
			
			/***************************2D version**************************
			//Generating Clock
			AsciiClock clock = new AsciiClock(50);			
			char[,] clockDisplay = clock.GetDisplay();
			
			//Making Print method for clock
			Action PrintClockMethod = ()=>
			{
				System.Text.StringBuilder sb = new StringBuilder();
				for(int i = 0; i < clockDisplay.GetLength(1); i++)
				{
					for(int j = 0; j < clockDisplay.GetLength(0); j++)
					{
						sb.Append(clockDisplay[j,i]);
					}
					sb.Append('\n');
				}
				Console.SetCursorPosition(0,0);
				Console.WriteLine(sb.ToString());	
			};
			clock.Start(PrintClockMethod);
			***************************************************************/
		}
		public static void DrawCube(char[,,] map)
		{
			int point_x = 10;
			int point_y = 10;
			int point_z = 10;
			
			int width = 15;
			int length = 15;
			int heigh = 15;
			
			for(int i = point_z; i < point_z + heigh; i++)
				for(int j = point_y; j <point_y + length; j++)
					for(int k = point_x; k < point_x + width; k++)
						map[i,j,k] = '_';
					
			for(int i = point_z; i < point_z + heigh; i++)
				map[i,point_y,point_x]
				=map[i,point_y+length,point_x]
				=map[i,point_y,point_x+width]
				=map[i,point_y+length,point_x+width]
				='a';
				
			for(int i = point_y; i < point_y + length; i++)
				map[point_z,i,point_x]
				=map[point_z+heigh,i,point_x]
				=map[point_z,i,point_x+width]
				=map[point_z+heigh,i,point_x+width]
				='a';
				
			for(int i = point_z; i < point_z + heigh; i++)
				map[point_z,point_y,i]
				=map[point_z+heigh,point_y,i]
				=map[point_z,point_y+length,i]
				=map[point_z+heigh,point_y+length,i]
				='a';
						
		}
		
	}
}