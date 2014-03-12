$files = Get-ChildItem ..\..\Src\DragqnLD\Tests\DragqnLD.Core.UnitTests\bin\Debug\Output\
$titles = @()
foreach ($file in $files)
{
	$data = Get-Content -Encoding UTF8 $file | Out-String | ConvertFrom-Json
	$titleValues = $data.http___linked_opendata_cz_ontology_drug_encyclopedia_title._value	
	foreach ($titleValue in $titleValues)
	{
		$titles += $titleValue
	}
}

