#!/bin/bash

if [ "$#" -ne 1 ]; then
	echo "You must specify the day to set up"
	exit 1
fi

DAY="$1"

cp -Rpf Template Day${DAY}.Part1
cp -Rpf Template Day${DAY}.Part2
mv Day${DAY}.Part1/Template.csproj Day${DAY}.Part1/Day${DAY}.Part1.csproj
mv Day${DAY}.Part2/Template.csproj Day${DAY}.Part2/Day${DAY}.Part2.csproj

dotnet sln add Day${DAY}.Part1
dotnet sln add Day${DAY}.Part2
