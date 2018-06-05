namespace Cam
{
	public class Model : CharacterBase
	{
		public float things;

		public override void Ability1()
		{
			base.Ability1();

			print("Merging");
			debugText = "Merging";
		}

		public override void Ability2()
		{
			base.Ability2();

			print("Split");
			debugText = "Split";
		}
	}

}