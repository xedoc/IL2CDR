# IL2CDR
IL2 Commander for BoS/BoM

This is a fork of the original repository https://github.com/xedoc/IL2CDR, but this sources have undertaken a massive update and COde CLeanup. 

On 11/2019, the whole project was widely updated:
* the project was updated to VS 2017 and is targetting the latest .NET (4.7.2)
* all sources have been Cleaned-up using ReSharper hints -- formatting, Coding Style, new C# language constructs, ... 
* all referenced libraries were updated
* maybe the most serious update was the upgrade of CS-Script -- upgraded to v3.30. From now, the Roslyn compiler is used, because it offers C# 7.x features. This may influence older scripts (e.g., namespace declaration is not allowed; anonymous types are sometimes problematic), but owerall, it offers better possibilities. 
* the PHP project was excluded from the common solution (will be managed separately)

