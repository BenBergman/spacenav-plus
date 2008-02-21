JNI_NAME = SpaceNav
JAVA_HOME = /usr/lib/jvm/java-6-sun-1.6.0.00/

SPNAV_CFLAGS = -I../libspnav/
SPNAV_LDFLAGS = -L../libspnav/ -lspnav -lX11


PACKAGE		:= net.sf.spacenav
JNI_DIR         := $(subst .,/,$(PACKAGE))
JAVA_SRC	:= $(wildcard $(JNI_DIR)/*.java)
JAVA_OBJS       := $(JAVA_SRC:.java=.class)

JNI_HEADER	:= $(subst .,_,$(PACKAGE))_$(JNI_NAME).h
NATIVE_LIB	:= lib$(JNI_NAME).so

all:	testjava $(NATIVE_LIB) $(JNI_NAME).jar documentation
testjava:
	@if [ ! -e $(JAVA_HOME) ]; then \
		echo "Please set JAVA_HOME to point to a SDK!"; \
		exit 1; \
	fi

documentation:
	$(JAVA_HOME)/bin/javadoc -d apidoc $(PACKAGE)

clean:
	rm -f $(JAVA_OBJS) \
	      $(JNI_HEADER) \
	      $(NATIVE_LIB) \
	      $(JNI_NAME).jar
	rm -Rf apidoc/

$(JNI_NAME).jar:	$(JAVA_OBJS)
	$(JAVA_HOME)/bin/jar cvf $@ $^


%.class:	%.java
	$(JAVA_HOME)/bin/javac $<

$(JNI_HEADER):	$(JNI_DIR)/$(JNI_NAME).class
	$(JAVA_HOME)/bin/javah -jni $(PACKAGE).$(JNI_NAME)

$(NATIVE_LIB):	$(JNI_HEADER) $(JNI_NAME).c
	gcc -I$(JAVA_HOME)/include \
	    -I$(JAVA_HOME)/include/linux \
	    $(SPNAV_LDFLAGS) \
	    $(SPNAV_CFLAGS) \
	    -shared \
	    -fPIC \
	    -o $@ $(JNI_NAME).c


.PHONY:	clean testjava documentation
