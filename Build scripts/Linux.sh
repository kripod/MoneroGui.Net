BASEDIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# Recreate output paths
rm -rf "Output"
rm -rf "Resources/Platform-independent/Localizations"
mkdir -p "Output/Linux/Resources"
mkdir -p "Output/Windows/Resources"
mkdir -p "Output/Mac"
mkdir -p "Resources/Platform-independent/Localizations"

# Build the project in Release mode
cd "../Source"
nuget restore "MoneroGui.Net.sln"
xbuild /p:Configuration=Release "MoneroGui.Net.sln"

# Remove debug databases
cd "MoneroGui.Net.Desktop/bin/Release"
find "." -name "*.mdb" -delete

OUTPUTDIRS=($(find -maxdepth 1 -type d -printf "%P\n"))

for i in "${OUTPUTDIRS[@]}"
do
	if [ "$i" == *.app ]; then
		# Clean and then move the Mac output directory
		find "./$i" -name "README*" -delete
		mv -f "$i" "$BASEDIR/Output/Mac"

	else
		# Move localization resource directory
		mv -f "$i" "$BASEDIR/Resources/Platform-independent/Localizations/$i"
	fi
done

# Copy files for Linux and Windows; create the Resources directory on Mac
cp -rf * "$BASEDIR/Output/Linux"
cp -rf * "$BASEDIR/Output/Windows"
mkdir -p "$BASEDIR/Output/Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/Resources"

# Copy the resources for each platform
cd "$BASEDIR/Resources/Platform-independent"
cp -rf * "$BASEDIR/Output/Linux/Resources"
cp -rf * "$BASEDIR/Output/Windows/Resources"
cp -rf * "$BASEDIR/Output/Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/Resources"

cd "$BASEDIR/Resources/Linux"
cp -rf * "$BASEDIR/Output/Linux/Resources"

cd "$BASEDIR/Resources/Windows"
cp -rf * "$BASEDIR/Output/Windows/Resources"

cd "$BASEDIR/Resources/Mac"
cp -rf * "$BASEDIR/Output/Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/Resources"

# Copy the readme and license files
cd "$BASEDIR/.."
EXTRAFILES=($(find -maxdepth 1 -type f -printf "%P\n"))

for i in "${EXTRAFILES[@]}"
do
	cp -rf "$i" "$BASEDIR/Output/Linux/$i"
	cp -rf "$i" "$BASEDIR/Output/Windows/$i"
	cp -rf "$i" "$BASEDIR/Output/Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/$i"
done

cp -rf "Licenses" "$BASEDIR/Output/Linux/Licenses"
cp -rf "Licenses" "$BASEDIR/Output/Windows/Licenses"
cp -rf "Licenses" "$BASEDIR/Output/Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/Licenses"

# Remove all the redundant libraries
cd "$BASEDIR/Output"
rm -f Linux/*Mac*.dll
rm -f Linux/*Wpf*.dll

rm -f Windows/*Mac*.dll
rm -f Windows/*Gtk*.dll

rm -f Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/*Wpf*.dll
rm -f Mac/MoneroGui.Net.Desktop.app/Contents/MonoBundle/*Gtk*.dll

# Pack every bundle
printf "\nApplication version: "
read VERSION

cd "Linux"
tar -cpzf "../MoneroGui.Net-v$VERSION-Linux-x64.tar.gz" *

cd "../Windows"
zip -r9 "../MoneroGui.Net-v$VERSION-Windows-x64.zip" *

cd "../Mac"
zip -r9 "../MoneroGui.Net-v$VERSION-Mac-x64.zip" *
