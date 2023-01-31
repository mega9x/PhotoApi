path=$(realpath $0)
cd "${path%/*}"
dotnet ./PhotoViewer.dll --urls $1