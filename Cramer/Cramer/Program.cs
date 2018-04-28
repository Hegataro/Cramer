/**
 * Created by SharpDevelop.
 * User: Hegataro
 * Date: 27.04.2018
 * Time: 13:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Cramer {
	/**
	 * 
	 */
	public class Equation {
		public List<decimal> Vars;
		public decimal Result;
		public Equation(int num, decimal result) {
			Vars=new List<decimal>();
			for (int i=0; i<num; i++) Vars.Add(0);
			Result=result;
		}
		/**
		 * Updates the Equation to a set size, removing or adding elements as seen fit
		 * input: the number to which the Equation's size should be set
		 */
		public void Update(int num) {
			if (Vars.Count>num) {
				while (Vars.Count>num) {
					Vars.RemoveAt(Vars.Count-1);
				}
			} else if (Vars.Count<num) {
				while (Vars.Count<num) {
					Vars.Add(0);
				}
			}
		}
	}
	
	/**
	 * 
	 */
	public class Matrix {
		public List<Equation> Equations;
		public decimal Determinant;
		public Matrix(int num) {
			Equations=new List<Equation>();
			for (int i=0; i<num; i++) Equations.Add(new Equation(num, 0));
		}
		/**
		 * Updates the Matrix to a set size, removing or adding elements as seen fit
		 * input: the number to which the Matrix' size should be set
		 */
		public void Update(int num) {
			if (Equations.Count>num) {
				while (Equations.Count>num) {
					Equations.RemoveAt(Equations.Count-1);
				}
				for (int i=0; i<Equations.Count; i++) Equations[i].Update(num);
			} else if (Equations.Count<num) {
				for (int i=0; i<Equations.Count; i++) Equations[i].Update(num);
				while (Equations.Count<num) {
					Equations.Add(new Equation(num, 0));
				}
			}
		}
	}
	
	/**
	 * 
	 */
	public class MatrixForm : Form {
		
		private Matrix CalculatedMatrix;
		private NumericUpDown SizeNumeric;
		private Button SizeButton;
		private Button GenButton;
		private Button CalcButton;
		private List<Label> VarLabels;
		private Label DeterminantLabel;
		private List<List<NumericUpDown>> Varboxes;
		private List<NumericUpDown> Resultboxes;
		
		public MatrixForm() {
			CalculatedMatrix=new Matrix(3);
			
			FormBorderStyle=FormBorderStyle.FixedSingle;
			
			SizeNumeric=new NumericUpDown {
				Location=new Point( 5, 5),
				Size=new Size( 65, 22),
				
				Value=3,
				Minimum=3,
				Maximum=10,
				Increment=1
			};
			
			SizeButton=new Button {
				Location=new Point( 5, SizeNumeric.Location.Y+23),
				Size=new Size( 65, 22),
				Text="Update"
			};
			SizeButton.Click+=SizeButton_Click;
			
			GenButton=new Button {
				Location=new Point( 5, SizeButton.Location.Y+23),
				Size=new Size( 65, 22),
				Text="Generate"
			};
			GenButton.Click+=GenButton_Click;
			
			CalcButton=new Button {
				Location=new Point( 5, GenButton.Location.Y+23),
				Size=new Size( 65, 22),
				Text="Calculate"
			};
			CalcButton.Click+=CalcButton_Click;
			
			DeterminantLabel=new Label {
				Location=new Point( 66*4+16, 5),
				Size=new Size( 65, 22),
				Text="0",
				TextAlign=ContentAlignment.MiddleCenter
			};
			VarLabels=new List<Label>();
			
			Varboxes=new List<List<NumericUpDown>>();
			Resultboxes=new List<NumericUpDown>();
			
			ResetUI(3);
			
			Controls.Add(SizeNumeric);
			Controls.Add(SizeButton);
			Controls.Add(GenButton);
			Controls.Add(CalcButton);
			Controls.Add(DeterminantLabel);
		}
		
		/**
		 * 
		 */
		private void TextConvert() {
			for (int i=0; i<Varboxes.Count; i++) {
				CalculatedMatrix.Equations[i].Result=Resultboxes[i].Value;
				for (int j=0; j<Varboxes[i].Count; j++) {
					CalculatedMatrix.Equations[i].Vars[j]=Varboxes[i][j].Value;
				}
			}
		}
		
		/**
		 * 
		 */
		private void ResetUI(int target) {
			int curr=Varboxes.Count;
			DeterminantLabel.Location=new Point( 66*(target+1)+21, 5);
			MaximumSize=new Size(66*(target+2)+31, 23*(target+2)+14);
			MinimumSize=new Size(66*(target+2)+31, 23*(target+2)+14);
			for (int i=0; i<curr; i++) {
				Controls.Remove(Resultboxes[curr-(1+i)]);
				Controls.Remove(VarLabels[curr-(1+i)]);
				for (int j=0; j<curr; j++) {
					Controls.Remove(Varboxes[curr-(1+i)][j]);
				}
				Varboxes[curr-(1+i)].Clear();
			}
			Resultboxes.Clear();
			VarLabels.Clear();
			Varboxes.Clear();
			
			for (int i=0; i<target; i++) {
				VarLabels.Add(new Label{ 
				              	Size=new Size( 65, 22), 
				              	Location=new Point( 66*(i+1)+11, 5), 
				              	TextAlign=ContentAlignment.MiddleCenter}
				              );
				Controls.Add(VarLabels[i]);
				Resultboxes.Add(new NumericUpDown{ 
				                	Size=new Size( 65, 22), 
				                	Location=new Point( 66*(target+1)+21, 23*(i+1)+5), 
				                	TextAlign=HorizontalAlignment.Right, 
				                	Minimum=-10000, 
				                	Maximum=10000, 
				                	Increment=1, 
				                	Value=CalculatedMatrix.Equations[i].Result}
				                );
				Controls.Add(Resultboxes[i]);
				Varboxes.Add(new List<NumericUpDown>());
			}
			for (int i=0; i<target; i++) {
				for (int j=0; j<target; j++) {
					Varboxes[i].Add(new NumericUpDown{ 
					                	Size=new Size( 65, 22), 
					                	Location=new Point( 66*(j+1)+11,23*(i+1)+5),
					                	TextAlign=HorizontalAlignment.Right, 
					                	Minimum=-10000, 
					                	Maximum=10000, 
					                	Increment=1, 
					                	Value=CalculatedMatrix.Equations[i].Vars[j]
					                });
					Controls.Add(Varboxes[i][j]);
				}
			}
			
			Calculate();
		}
		
		/**
		 * 
		 */
		private void Calculate() {
			TextConvert();
			for (int i=0; i<CalculatedMatrix.Equations.Count; i++) {
				VarLabels[i].Text=CalcVar(i).ToString();
				
			}
			DeterminantLabel.Text=CalcDeterminant(CalculatedMatrix).ToString();
		}
		
		/**
		 * 
		 */
		private void SizeButton_Click(object sender, EventArgs e) {
			CalculatedMatrix.Update(Decimal.ToInt32(SizeNumeric.Value));
			ResetUI(Decimal.ToInt32(SizeNumeric.Value));
		}
		
		/**
		 * 
		 */
		private void GenButton_Click(object sender, EventArgs e) {
			Random rand=new Random();
			for (int i=0; i<Varboxes.Count; i++) {
				Resultboxes[i].Value=rand.Next(201)-100;
				for (int j=0; j<Varboxes.Count; j++) {
					Varboxes[i][j].Value=rand.Next(201)-100;
				}
			}
			Calculate();
			for (int i=0; i<VarLabels.Count; i++) {
				Console.WriteLine("{0}: {1}", i, VarLabels[i].Text);
			}
			Console.WriteLine("D: {0}", DeterminantLabel.Text);
			Console.WriteLine("___________________________________________________________________");
		}
		
		/**
		 * 
		 */
		private void CalcButton_Click(object sender, EventArgs e) {
			Calculate();
		}
		
		/**
		 * 
		 */
		private decimal CalcDeterminant(Matrix matrix) {
			decimal returned=0;
			decimal current=1;
			
			for (int i=0; i<matrix.Equations.Count*2; i++) {
				current=1;
				if (i<matrix.Equations.Count) {
					for (int j=0; j<matrix.Equations.Count; j++) {
						current*=matrix.Equations[(i+j) % matrix.Equations.Count].Vars[j];
					}
					
					returned+=current;
				} else {
					for (int j=0; j<matrix.Equations.Count; j++) {
						current*=matrix.Equations[(i+j) % matrix.Equations.Count].Vars[matrix.Equations.Count-(j+1)];
					}
					returned-=current;
				}
			}
			return returned;
		}
		
		/**
		 * 
		 */
		private decimal CalcVar(int num) {
			Matrix matrix=new Matrix(CalculatedMatrix.Equations.Count);
			
			for (int i=0; i<CalculatedMatrix.Equations.Count; i++) {
				matrix.Equations[i].Result=CalculatedMatrix.Equations[i].Result;
				for (int j=0; j<CalculatedMatrix.Equations[i].Vars.Count; j++) {
					matrix.Equations[i].Vars[j]=CalculatedMatrix.Equations[i].Vars[j];
				}
			}
			
			decimal determinant=CalcDeterminant(matrix);
			if (determinant!=0) {
				for (int i=0; i<matrix.Equations.Count; i++) {
					matrix.Equations[i].Vars[num]=matrix.Equations[i].Result;
				}
				return CalcDeterminant(matrix)/determinant;
			} else {
				return 0;
			}
		}
	}
	
	class Program {
		public static void Main(string[] args) {
			MatrixForm form=new MatrixForm();
			Application.EnableVisualStyles();
			Application.Run(form);
		}
	}
}