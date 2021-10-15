CSC = mcs
MONO = mono

test2: test2RngStream.exe test2RngStream.res
	$(MONO) test2RngStream.exe | tee test.res
	@echo Check whether there are any differences from the reference test output:
	diff -u test2RngStream.res test.res
	rm test.res # tidy up

test: testRngStream.exe
	$(MONO) testRngStream.exe

test2RngStream.exe: test2RngStream.cs rngstream.cs
	$(CSC) test2RngStream.cs rngstream.cs

testRngStream.exe: testRngStream.cs rngstream.cs
	$(CSC) testRngStream.cs rngstream.cs

test2dotnet:
	dotnet clean
	dotnet build /p:StartupObject=test2RngStream
	dotnet run

testdotnet:
	dotnet clean
	dotnet build /p:StartupObject=testRngStream
	dotnet run

clean:
	rm -f test.res
	rm -f test2RngStream.exe
	rm -f testRngStream.exe

