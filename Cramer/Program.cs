/**
 * \mainpage Programová dokumentace 
 * \author Hegataro
 * \date 27.04.2018
 * \detail Zadání: Cramerovo pravidlo - konzolová aplikace s následujícími vlastnostmi
 *   - Uživatel může nastavit velikost a parametry matice
 *   - Uživatel může náhodně vygenerova čísla v matici
 *   - Výsledek determinantu matice
 *
 * Zdrojové kódy jsou zapsány ve znakové sadě UTF-8
 * */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Cramer {
	/**
	 * \brief Třída Equation charakterizující rovnice
	 */
	public class Equation {
		/**
		 * \property Vars Seznam proměnných
	 	 * \property Result Výsledek
	     */
		public List<double> Vars;
		public double Result;
		/**
		 * \brief Konstruktor třídy Equation
		 * \param num Počet promněnných
		 * \param result Výsledek
		 */
		public Equation(int num, double result) {
			Vars=new List<double>();
			for (int i=0; i<num; i++) Vars.Add(0);
			Result=result;
		}
		/**
		 * \brief Metoda Update určená k aktualizaci matice na nastavenou velikost, odebírá nebo přidává prvky podle potřeby
		 * \param num Počet promněnných
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
	 * \brief Třída charakterizující matici
	 */
	public class Matrix {
		public List<Equation> Equations; /** Seznam rovnic*/
		/**
		 * \brief Konstruktor třídy Matrix
		 * \param num Počet promněnných
		 */
		public Matrix(int num) {
			Equations=new List<Equation>();
			for (int i=0; i<num; i++) Equations.Add(new Equation(num, 0));
		}
		/**
		 * \brief Metoda Update určená k aktualizaci matice na nastavenou velikost, odebírá nebo přidává prvky podle potřeby
		 * \param num Počet promněnných
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
	 * \brief Třída pro uživatelské rozhraní
	 */
	public class MatrixForm : Form {
		
		private Matrix CalculatedMatrix;
		private NumericUpDown SizeNumeric;
		private Button GenButton;
		private List<Label> VarLabels;
		private Label DeterminantLabel;
		private List<List<NumericUpDown>> Varboxes;
		private List<NumericUpDown> Resultboxes;
		/**
		 * \brief Konstruktor třídy MatrixForm
		 */
		public MatrixForm() {
			
			
			CalculatedMatrix=new Matrix(3);
			
			Text="Cramer's Rule Calculator";
			FormBorderStyle=FormBorderStyle.FixedSingle;
			
			SizeNumeric=new NumericUpDown {
				Location=new Point( 5, 5),
				Size=new Size( 65, 22),
				
				Value=3,
				Minimum=3,
				Maximum=25,
				Increment=1
			};
			SizeNumeric.ValueChanged+=Numeric_Change;
			
			
			GenButton=new Button {
				Location=new Point( 5, SizeNumeric.Location.Y+23),
				Size=new Size( 65, 22),
				Text="Generate"
			};
			GenButton.Click+=GenButton_Click;
			
			DeterminantLabel=new Label {
				Location=new Point( 66*4+16, 5),
				Size=new Size( 65, 22),
				Text="0",
				TextAlign=ContentAlignment.MiddleCenter
			};
			VarLabels=new List<Label>();
			
			Varboxes=new List<List<NumericUpDown>>();
			Resultboxes=new List<NumericUpDown>();
			
			for (int i=0; i<SizeNumeric.Maximum; i++) {
				VarLabels.Add(new Label{ 
				              	Size=new Size( 65, 22), 
				              	Location=new Point( 66*(i+1)+11, 5), 
				              	TextAlign=ContentAlignment.MiddleCenter}
				              );
				
				Resultboxes.Add(new NumericUpDown{ 
				                	Size=new Size( 65, 22), 
				                	Location=new Point( 0, 23*(i+1)+5), 
				                	TextAlign=HorizontalAlignment.Right, 
				                	Minimum=-10000, 
				                	Maximum=10000, 
				                	Increment=1, 
				                	Value=0}
				                );
				Resultboxes[Resultboxes.Count-1].ValueChanged+=Numeric_Change;
				Varboxes.Add(new List<NumericUpDown>());
				for (int j=0; j<SizeNumeric.Maximum; j++) {
					Varboxes[i].Add(new NumericUpDown{ 
					                	Size=new Size( 65, 22), 
					                	Location=new Point( 66*(j+1)+11,23*(i+1)+5),
					                	TextAlign=HorizontalAlignment.Right, 
					                	Minimum=-10000, 
					                	Maximum=10000, 
					                	Increment=1, 
					                	Value=0
					                });
					Varboxes[i][Varboxes[i].Count-1].ValueChanged+=Numeric_Change;
				}
			}
			ResetUI( (int)SizeNumeric.Maximum, 0);
			ResetUI( (int)SizeNumeric.Minimum, (int)SizeNumeric.Maximum);
			
			Controls.Add(SizeNumeric);
			Controls.Add(GenButton);
			Controls.Add(DeterminantLabel);
		}
		
		/**
		 * \brief Metoda TextConvert konvertuje obsah textboxů do matice
		 */
		private void TextConvert() {
			for (int i=0; i<CalculatedMatrix.Equations.Count; i++) {
				CalculatedMatrix.Equations[i].Result=(double)Resultboxes[i].Value;
				for (int j=0; j<CalculatedMatrix.Equations[i].Vars.Count; j++) {
					CalculatedMatrix.Equations[i].Vars[j]=(double)Varboxes[i][j].Value;
				}
			}
		}
		
		/**
		 * \brief Metoda ResetUI zmenšuje a zvětšuje matici
		 * \param target Požadovaná velikost matice
		 * \param curr Nynější velikost matice
		 */
		private void ResetUI(int target, int curr) {
			DeterminantLabel.Location=new Point( 66*(target+1)+21, 5);
			MaximumSize=new Size(66*(target+2)+31, 23*(target+2)+14);
			MinimumSize=new Size(66*(target+2)+31, 23*(target+2)+14);
			if (curr>target) {
				for (int i=0; i<curr; i++) {
					if (i>target-1) {
						Controls.Remove(Resultboxes[i]);
						Controls.Remove(VarLabels[i]);
					}
					for (int j=0; j<curr; j++) {
						if (i>(target-1) || j>(target-1)) {
							Controls.Remove(Varboxes[i][j]);
						}
					}
				}
			} else if (curr<target) {
				for (int i=0; i<target; i++) {
					if (!Controls.Contains(Resultboxes[i])) {
						Controls.Add(Resultboxes[i]);
					}
					if (!Controls.Contains(VarLabels[i])) {
						Controls.Add(VarLabels[i]);
					}
				}
				for (int i=0; i<target; i++) {
					for (int j=0; j<target; j++) {
						if (!Controls.Contains(Varboxes[i][j])) {
							Controls.Add(Varboxes[i][j]);
						}
					}
				}
			}
			for (int i=0; i<(int)SizeNumeric.Value; i++) {
				Resultboxes[i].Location=new Point(66*(target+1)+21, Resultboxes[i].Location.Y);
			}
			
			Calculate();
		}
		
		/**
		 * \brief Metoda Calculate počítá promněnné a determinant
		 */
		private void Calculate() {
			TextConvert();
			for (int i=0; i<CalculatedMatrix.Equations.Count; i++) {
				VarLabels[i].Text=Convert(CalcVar(i));
				
			}
			DeterminantLabel.Text=Convert(CalcDeterminant(CalculatedMatrix));
		}
		
		/**
		 * \brief Metoda Convert konvertuje čísla, které jsou příliš malé nebo příliš velké
		 * \param input Číslo, které se konvertuje
		 * \return Vrací konvertované číslo ve formě string
		 */
		private string Convert(double input) {
			int i=0;
			if (input!=0 && (Math.Abs(input)>=1000 || Math.Abs(input)<=0.001)) {
				if (Math.Abs(input)<1) {
					while (Math.Abs(input)<1) {
						input*=10;
						i--;
					}
				} else if (Math.Abs(input)>10) {
					while (Math.Abs(input)>10) {
						input/=10;
						i++;
					}
				}
			}
			
			if (i==0) {
				return Math.Round(input, 3).ToString();
			} else if (i>0) {
				return Math.Round(input, 3).ToString()+"E+"+i.ToString();
			} else {
				return Math.Round(input, 3).ToString()+'E'+i.ToString();
			}
		}
		
		/**
		 * \brief Metoda GenButton_CLick generuje náhodné hodnoty
		 */
		private void GenButton_Click(object sender, EventArgs e) {
			GenButton.Enabled=false;
			Random rand=new Random();
			for (int i=0; i<Varboxes.Count; i++) {
				Resultboxes[i].Value=rand.Next(201)-100;
				for (int j=0; j<Varboxes.Count; j++) {
					Varboxes[i][j].Value=rand.Next(201)-100;
				}
			}
			Calculate();
			GenButton.Enabled=true;
		}
		
		/**
		 * \brief Metoda Numeric_Change aktualizuje celou matici
		 */
		private void Numeric_Change(object sender, EventArgs e) {
			int temp=CalculatedMatrix.Equations.Count;
			CalculatedMatrix.Update(Decimal.ToInt32(SizeNumeric.Value));
			ResetUI(Decimal.ToInt32(SizeNumeric.Value), temp);
		}
		
		/**
		 * \brief Metoda CalcDeterminant vypočítá determinant
		 * \param matrix Matice, která se počítá
		 * \return Vrací determinant zadané matice
		 */
		private double CalcDeterminant(Matrix matrix) {
			double returned=0;
			double current=1;
			
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
		 * \brief Metoda CalcVar vypočítá jednu promněnnou
		 * \param num Kolikátá proměnná se počítá
		 * \return Vrací hodnotu proměnné
		 */
		private double CalcVar(int num) {
			Matrix matrix=new Matrix(CalculatedMatrix.Equations.Count);
			
			for (int i=0; i<CalculatedMatrix.Equations.Count; i++) {
				matrix.Equations[i].Result=CalculatedMatrix.Equations[i].Result;
				for (int j=0; j<CalculatedMatrix.Equations[i].Vars.Count; j++) {
					matrix.Equations[i].Vars[j]=CalculatedMatrix.Equations[i].Vars[j];
				}
			}
			
			double determinant=CalcDeterminant(matrix);
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
	/*
	 * \brief Hlavní funkce programu
	 */
	class Program {
		public static void Main(string[] args) {
			MatrixForm form=new MatrixForm();
			Application.EnableVisualStyles();
			Application.Run(form);
		}
	}
}
