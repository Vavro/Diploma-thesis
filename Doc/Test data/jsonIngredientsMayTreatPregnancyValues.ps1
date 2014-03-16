$files = Get-ChildItem ..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\Ingredients

$lines = @()
foreach ($file in $files)
{
	$line = ""
	$fileName = "..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\Ingredients\" + $file
	$data = Get-Content -Encoding UTF8 $fileName | Out-String | ConvertFrom-Json
	$mayTreatValues = $data.http___linked_opendata_cz_ontology_drug_encyclopedia_mayTreat.http___linked_opendata_cz_ontology_drug_encyclopedia_title._value	
	$pregnancyCategories = $data.http___linked_opendata_cz_ontology_drug_encyclopedia_hasPregnancyCategory
	foreach ($mayTreatValue in $mayTreatValues)
	{
		if ($line)
		{
			$line += ";;"
		}
		$line += "mt: " + $mayTreatValue
	}
	
	foreach ($pregnancyCategory in $pregnancyCategories)
	{
		if ($line)
		{
			$line += ";;"
		}
		$line += "pc: " + $pregnancyCategory
	}
	
	if($pregnancyCategories.Count)
	{
		if ($MayTreatValues.Count)
		{
			$lines += $line
		}
	}
}
echo $lines | Out-File "IngredientsMayTreatPregnancy.txt"
