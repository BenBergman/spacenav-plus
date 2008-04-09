CLASSPATH	:= ../SpaceNav.jar:.
JAVA_LIB_PATH	:= ../
LD_LIBRARY_PATH := ../../libspnav

all:	Test.class

run:	Test.class
	LD_LIBRARY_PATH=$(LD_LIBRARY_PATH) java -cp $(CLASSPATH) -Djava.library.path=$(JAVA_LIB_PATH) Test

clean:
	rm -f Test.class

Test.class: Test.java ../SpaceNav.jar

%.class:	%.java
	javac -cp $(CLASSPATH) $<

.PHONY:	clean run
