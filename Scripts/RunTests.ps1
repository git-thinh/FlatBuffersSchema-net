$SolutionDir = "$PSScriptRoot\.."

$Runner = "$SolutionDir\packages\NUnit.ConsoleRunner.3.4.1\tools\nunit3-console.exe"

$TestExe = "$SolutionDir\FlatBuffersSchemaTests\bin\Debug\FlatBuffersSchemaTests.dll"

& $Runner $TestExe

pause