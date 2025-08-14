watch:
	dotnet watch run --project ./Fe.PatchHelper/Fe.PatchHelper.csproj
run:
	dotnet run --project ./Fe.PatchHelper/Fe.PatchHelper.csproj
watch-test:
	echo "no test project set up yet"
outdated:
	dotnet outdated -exc FluentAssertions
outdated-upgrade:
	dotnet outdated -exc FluentAssertions -u