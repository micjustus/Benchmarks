# Description
This repository contains one or more projects that test the performance of different types, or custom approaches using the beautiful BenchmarkDotNet Nuget package.

# Benchmarks

## DateTimeParser

### Overview
This benchmark tests the *TryParse*, *TryParseExtact*, *Regular Expression Parsing* and *Custom Parsing* approaches to parsing a string *DateTime* instance.

### Performance
As can be seen from the Release build run, the custom parser approach is the fastest. 
*Why?* Because it focuses on several key points
- Using *ReadOnlySpan<char>* to minimise allocations
- Reduce any dependence on method calls, like *char.IsDigit(...)*
- Makes a **strong** assumption about the incoming format. (This is the weakest point for this approach)
   
![image](https://github.com/user-attachments/assets/b824bfc7-546b-46fe-bb01-973db289968e)


