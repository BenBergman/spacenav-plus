/*
This file is part of libspnav, part of the spacenav project (spacenav.sf.net)
Copyright (C) 2007 John Tsiombikas <nuclear@siggraph.org>
Copyright (C) 2008 Michael Arndt <scriptkiller@gmx.de>

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
OF SUCH DAMAGE.
*/

#include <jni.h>
#include <stdio.h>
#include "net_sf_spacenav_SpaceNav.h"

#include <spnav.h>


#ifdef DEBUG
#define debug_print(str) fprintf(stderr, "%s:%d " str "\n", __FILE__, __LINE__)
#define debug_printf(fmt, ...) fprintf(stderr, "%s:%d " fmt "\n", __FILE__, __LINE__, __VA_ARGS__)
#else
#define debug_print(str)
#define debug_printf(fmt, ...)
#endif

JNIEXPORT void JNICALL Java_net_sf_spacenav_SpaceNav_open(JNIEnv *env, jobject obj) {
  debug_print("Java_SpaceNav_open called");
  if(spnav_open()!=0) {
    debug_print("spnav_open() failed");
  }
  debug_print("Java_SpaceNav_open returning");
}

JNIEXPORT void JNICALL Java_net_sf_spacenav_SpaceNav_close(JNIEnv *env, jobject obj) {
  debug_print("Java_SpaceNav_close called");
  spnav_close();
  debug_print("Java_SpaceNav_close returning");
}

JNIEXPORT void JNICALL Java_net_sf_spacenav_SpaceNav_sensitivity(JNIEnv *env,
    jobject obj, jdouble value) {
  debug_print("Java_SpaceNav_sensitivity called");
  if(spnav_sensitivity(value)!=0) {
    debug_print("spnav_sensitivity() failed");
  }
  debug_print("Java_SpaceNav_sensitivity returning");
}


JNIEXPORT jobject JNICALL Java_net_sf_spacenav_SpaceNav_wait_1event(JNIEnv *env, jobject obj) {

  jclass eventClass;
  jmethodID cid;
  jstring result;

  spnav_event e;
  struct spnav_event_motion *me=&e.motion;
  struct spnav_event_button *be=&e.button;

  debug_print("Java_SpaceNav_wait_1event called");

  if(spnav_wait_event(&e)==0) {
    debug_print("spnav_wait_event() failed");
    return NULL;
  }


  if(e.type == SPNAV_EVENT_MOTION) {

    eventClass = (*env)->FindClass(env, "net/sf/spacenav/SpaceNavMotionEvent");
    if (eventClass == NULL) {
      return NULL; /* exception thrown */
    }

    cid = (*env)->GetMethodID(env, eventClass,
	"<init>", "(IIIIII)V");
    if (cid == NULL) {
      return NULL; /* exception thrown */
    }

    /* Construct a new object */
    result = (*env)->NewObject(env, eventClass, cid,
	me->x, me->y, me->z,
	me->rx, me->ry, me->rz /*, me->period*/);
    /* Free local references */
    (*env)->DeleteLocalRef(env, eventClass);
    return result;


  }
  else if(e.type == SPNAV_EVENT_BUTTON) {

    eventClass = (*env)->FindClass(env, "net/sf/spacenav/SpaceNavButtonEvent");
    if (eventClass == NULL) {
      return NULL; /* exception thrown */
    }

    cid = (*env)->GetMethodID(env, eventClass,
	"<init>", "(IZ)V");
    if (cid == NULL) {
      return NULL; /* exception thrown */
    }

    /* Construct a new object */
    result = (*env)->NewObject(env, eventClass, cid,
	(jint)be->bnum, (jint)be->press);
    /* Free local references */
    (*env)->DeleteLocalRef(env, eventClass);
    return result;
  }
  else {
    debug_printf("Unknown event-type: %d", e.type);
  }

  debug_print("Java_SpaceNav_wait_1event returning");
}

