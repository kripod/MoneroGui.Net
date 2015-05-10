BASEDIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

# Build the project in Release mode
cd ../Source
nuget restore MoneroGui.Net.sln
xbuild /p:Configuration=Release MoneroGui.Net.sln
cd MoneroGui.Net.Desktop/bin/Release

# Remove debug databases
rm -f *.mdb

mkdir -p "$BASEDIR/Output"
mkdir -p "$BASEDIR/Resources/Localizations"

OUTPUTDIRS=($(find -maxdepth 1 -type d -printf '%P\n'))

for i in "${OUTPUTDIRS[@]}"
do
	if [ "$i" == *.app ]; then
		# Move the Mac output directory
		mv -f "$i" "$BASEDIR/Output/MoneroGui.Net-v-Mac-x64.tar.gz"

		find "./$i" -name "README* *.mdb" -delete

	else
		# Move the localization resource directory
		mv -f "$i" "$BASEDIR/Resources/Localizations/$i"
	fi
done


tar -cpzf "$BASEDIR/Output/MoneroGui.Net-v-Linux-x64.tar.gz" *
