#region + Using Directives

#endregion

// user name: jeffs
// created:   2/4/2024 6:57:25 AM

namespace SharedCode.ShDataSupport
{

	/*
	public class PdfText1
	{


		public PdfText1()
		{

		}

		public void TestGetText(PdfDocument pdfDoc)
		{
			List<TextBit> textBits;

			PdfPage page = pdfDoc.GetPage(1);

			textBits = GetTextOnPage(page);

			ShowTextBitInfo(textBits);

			// DrawBoxes(page, textBits);

		}


		public List<TextBit> GetTextOnPage(PdfPage page)
		{
			return PdfTextAndLocationStrategy.GetTextBits(page);

		}

		public void ShowTextBitInfo(List<TextBit> textBits)
		{
			Console.WriteLine($"bits found| {textBits.Count}");

			foreach (TextBit tb in textBits)
			{
				ITextChunkLocation l = tb.Location;
				Vector vs = l.GetStartLocation();
				Vector ns = vs.Normalize();
				Vector ve = l.GetEndLocation();
				Vector ne = ve.Normalize();

				Rectangle r = tb.BoundBox;

				Rectangle st = tb.TopLineSegment.GetBoundingRectangle();
				Rectangle sb = tb.BottomLineSegment.GetBoundingRectangle();


				
				 //* notes: the top line segment and the bottom line segment appear to provide
				 //* the coorect coordinates for the (4) corners of the bounding box
				

				Console.WriteLine($"\ntext| {tb.Text}");
				Console.WriteLine($"box| X, Y, H, W       | {r.GetX()}, {r.GetY()}, {r.GetHeight()}, {r.GetWidth()}");
				Console.WriteLine($"box| L, T, R, B       | {r.GetLeft()}, {r.GetTop()}, {r.GetRight()}, {r.GetBottom()}");

				Console.WriteLine($"top line s| X, Y, H, W| {st.GetX()}, {st.GetY()}, {st.GetHeight()}, {st.GetWidth()}");
				Console.WriteLine($"top line s| L, T, R, B| {st.GetLeft()}, {st.GetTop()}, {st.GetRight()}, {st.GetBottom()}");

				Console.WriteLine($"bot line s| X, Y, H, W| {sb.GetX()}, {sb.GetY()}, {sb.GetHeight()}, {sb.GetWidth()}");
				Console.WriteLine($"bot line s| L, T, R, B| {sb.GetLeft()}, {sb.GetTop()}, {sb.GetRight()}, {sb.GetBottom()}");


				Console.WriteLine($"loc| dist para end    | {l.DistParallelEnd()}");
				Console.WriteLine($"loc| dist para start  | {l.DistParallelStart()}");
				Console.WriteLine($"loc| dist perp        | {l.DistPerpendicular()}");
				Console.WriteLine($"loc| start vect len   | {vs.Length()}");
				Console.WriteLine($"loc| start vect       | {vs.Get(0)} ,  {vs.Get(1)} ,  {vs.Get(2)}");
				Console.WriteLine($"loc| start norm vect  | {ns.Get(0)} ,  {ns.Get(1)} ,  {ns.Get(2)}");
				Console.WriteLine($"loc|   end vect len   | {ve.Length()}");
				Console.WriteLine($"loc|   end vect       | {ve.Get(0)} ,  {ve.Get(1)} ,  {ve.Get(2)}");
				Console.WriteLine($"loc|   end norm vect  | {ne.Get(0)} ,  {ne.Get(1)} ,  {ne.Get(2)}");
			}
		}

		public void DrawBoxes(PdfPage page, List<TextBit> textBits)
		{
			PdfCanvas canvas = new PdfCanvas(page);

			Color red = new DeviceRgb(255, 0, 0);

			ITextChunkLocation l = textBits[0].Location;

			canvas.SaveState();

			canvas
			.SetStrokeColor(red)
			.SetFillColor(red)
			.SetExtGState(new PdfExtGState().SetFillOpacity(0.3f))
			.Rectangle(150, 600, 250, 150)
			.FillStroke();

			canvas.RestoreState();

		}


		public override string ToString()
		{
			return $"this is {nameof(PdfText1)}";
		}
	}

		class PdfTextAndLocationStrategy : LocationTextExtractionStrategy
	{
		public List<TextBit> ResultCoordinates { get; set; }


		public PdfTextAndLocationStrategy()
		{
			ResultCoordinates = new List<TextBit>();
		}

		public static List<TextBit> GetTextBits(PdfPage page)
		{
			PdfTextAndLocationStrategy strategy = new PdfTextAndLocationStrategy();
			PdfTextExtractor.GetTextFromPage(page, strategy);

			return strategy.ResultCoordinates;
		}

		public override void EventOccurred(IEventData data, EventType type)
		{
			if (!type.Equals(EventType.RENDER_TEXT)) return;

			string text;
			ITextChunkLocation t;

			TextRenderInfo ri = (TextRenderInfo) data ;
			// CharacterRenderInfo a;
			
			

			// string text = ri.GetText();
			// CharacterRenderInfo b = new CharacterRenderInfo(ri);
			// ResultCoordinates.Add(new TextBit(text, b.GetLocation()));

			// Rectangle rb = b.GetBoundingBox();

			// Rectangle rFinal = new Rectangle(0, 0, 0, 0);

			// Console.WriteLine("\nnew subject");
			// Console.WriteLine($"base text| {ri.GetText()}\n");
			// a = new CharacterRenderInfo(ri);

			CharacterRenderInfo b = new CharacterRenderInfo(ri);
			IList<TextRenderInfo> ris = ri.GetCharacterRenderInfos();
			text=ri.GetText();
			
			t = b.GetLocation();
			Rectangle r = b.GetBoundingBox();

			LineSegment top = ri.GetAscentLine();
			LineSegment bott = ri.GetDescentLine();



			ResultCoordinates.Add(new TextBit(text, t, r, top, bott));

			return;
			

			
			//for (var i = 0; i < ris.Count; i++)
			//{
			//	text = ris[i].GetText();
			//	a = new CharacterRenderInfo(ris[i]);

			//	t = a.GetLocation();

			//	ResultCoordinates.Add(new TextBit(text, t));

			//	// rFinal = addHorizontal(rFinal, r);
			//}
			
			//// CharacterRenderInfo c = new CharacterRenderInfo(ris[0]);
			
			
			

		}

		private Rectangle addHorizontal(Rectangle rOrig, Rectangle rAdd)
		{
			float x;
			float y;
			float h;
			float w = 0.0f;

			if (rOrig.GetHeight() == 0.0f)
			{
				x = rAdd.GetX();
				y = rAdd.GetY();
				h = rAdd.GetHeight();
			}

			x = rOrig.GetX() < rAdd.GetX() ? rOrig.GetX() : rAdd.GetX();
			y = rOrig.GetY() > rAdd.GetY() ? rOrig.GetY() : rAdd.GetY();
			h = rOrig.GetHeight() > rAdd.GetHeight() ? rOrig.GetHeight() : rAdd.GetHeight();
			w = rOrig.GetWidth() + rAdd.GetWidth();

			return new Rectangle(x, y, w, h);

		}

	}


/*
	public class TextBit 
	{
		public string Text { get; set; }
		// public Rectangle ResultCoordinates { get; set; }
		public ITextChunkLocation Location { get; set; }
		public Rectangle BoundBox { get; set; }

		public LineSegment TopLineSegment { get; set; }
		public LineSegment BottomLineSegment { get; set; }
		
		
		public TextBit(string s, ITextChunkLocation l, 
			Rectangle box,
			LineSegment top,
			LineSegment bottom
			) 
		{
			Text = s;
			BoundBox = box;
			Location = l;

			TopLineSegment = top;
			BottomLineSegment = bottom;
		}
	}
*/
}
