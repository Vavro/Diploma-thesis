$files = Get-ChildItem ..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\MedicinalProducts
$titles = @()
foreach ($file in $files)
{
	$fileName = "..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\MedicinalProducts\" + $file
	$data = Get-Content -Encoding UTF8 $fileName | Out-String | ConvertFrom-Json
	$titleValues = $data.http___linked_opendata_cz_ontology_drug_encyclopedia_title._value	
	foreach ($titleValue in $titleValues)
	{
		$titles += $titleValue
	}
}
echo $titles | Out-File "MedicinalProductsTitles.txt"
