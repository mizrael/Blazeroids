solutionName="Blazeroids"
projectName="Blazeroids.Web"
projectFile="$projectName/$projectName.csproj"

rm -rf "build"

echo "building $projectName ..."
buildPath="build/$projectName-tmp"    
dotnet publish $projectFile --output $buildPath --configuration Debug
mv $buildPath/wwwroot/ "build/"        
rm -rf $buildPath

sed -i -e "s/<base href=\"\/\" \/>/<base href=\"\/$solutionName\/\" \/>/g" build/wwwroot/index.html

cp readme.md build/readme.md