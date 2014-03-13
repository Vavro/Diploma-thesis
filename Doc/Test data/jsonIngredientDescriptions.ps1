$files = Get-ChildItem ..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\Ingredients
$descriptions = @()
foreach ($file in $files)
{
	$fileName = "..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\Ingredients\" + $file
	$data = Get-Content -Encoding UTF8 $fileName | Out-String | ConvertFrom-Json
	$descriptionValues = $data.http___linked_opendata_cz_ontology_drug_encyclopedia_description._value	
	foreach ($descriptionValue in $descriptionValues)
	{
		$descriptions += $descriptionValue
	}
}
echo $descriptions | Out-File "IngredientDescriptions.txt"
