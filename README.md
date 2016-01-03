#ReleaseNameGenerator
Inspired by: https://github.com/tkambler/grunt-release-name-generator

##Overview
Generates a random unique release name along the following pattern [adjective] [animal].
Each combination generated is stored in order to garentue unique generation.

##Options

`-o` Sets the default category to be used for all enviroments **Can only be specified once**

`-e` Defines an enviroment for release name to be generated for. An optional category can be specified.

`-h` Prints the help dialog

##Examples
```
ReleaseNameGenerator.exe
Generated Dental Dolphin
```

```
ReleaseNameGenerator.exe -o Lizards
Generated Ill Iguana
```

```
ReleaseNameGenerator.exe -o Lizards -e Staging Birds -e Release
Generated Criminal Cuckoo for Staging
Generated Mammoth Monitor for Release
```
