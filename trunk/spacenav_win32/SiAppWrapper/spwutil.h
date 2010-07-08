/*-----------------------------------------------------------------------------
 *
 *  spw_util.h: SpaceWare Library Integration Module (SLIM) util definitions 
 *
<<<<<<< spwutil.h
 *  $Id: spwutil.h,v 1.3 2001/01/16 01:18:40 HJin Exp $
=======
 *  $Id: spwutil.h,v 1.3 2001/01/16 01:18:40 HJin Exp $
>>>>>>> 1.1.1.1.4.1
 *
 *-----------------------------------------------------------------------------
 */

#ifndef _SPW_UTIL_H_
#define _SPW_UTIL_H_ 1
#include <stdlib.h>

#ifdef __cplusplus
extern "C" {
#endif

/* PUBLIC VARIABLES & DEFINES */

#define SPW_INITIAL_TRANS_SCALE 1.0f
#define SPW_INITIAL_ROT_SCALE   1.0f
extern float SPW_TransScale;
extern float SPW_RotScale;

#define SPW_DOMINANT           (1)/* Possible bit masks for SPW_DOFFilterMode */
#define SPW_TRANS_OFF          (1<<1)
#define SPW_ROT_OFF            (1<<2)
#define SPW_TRANS_Z_ONLY       (1<<3)
extern int SPW_DOFFilterMode;

/* This variable points to a function that is called when the user lets go of
   the Spaceball.  It is typically used to draw the geometry at an increased
   "resolution" (e.g. shaded instead of wireframe, finer tesselation level,
   ... ) something like that that takes too much time to be done interactively
   on the current machine.  The idea being, that a user desires interactivity
   more than perfect detail while they are moving things around, but want it
   rendered "pretty" when they let go.

   If set, this function gets called from the "view" functions when the 
   user lets go of the ball.
*/
extern void (*SPW_BallReleased)();


/* Every viewing mode function takes an address of a ptr to one of these 
   spw_State_st structures.  Initially, the ptr is NULL, and is assigned 
   by the first viewing function that is called with it.
*/
typedef struct spw_State_st *SPW_State_t;

int SPW_Object (Matrix, float[7], Matrix,float[3], int, SPW_State_t*);
int SPW_Eyepoint (Matrix, float[7], Matrix,int, SPW_State_t*);
int SPW_Orbit (Matrix, float[7], Matrix,float[3], float[3], 
                          int, SPW_State_t*);
int SPW_OrbitDirection (Matrix, float[7], Matrix, float[3], float[3], float[3],
                        int, SPW_State_t*);
void SPW_OrbitParametersFromMatrix( float *pxrot, float *pyrot, float *pradius,
                                  Matrix world_to_eye, float orbit_pt[3],
                                  SPW_State_t *pstate );
int SPW_KeepInClipVolume (float [3],float ,float ,float ,float);
int SPW_ClipXY (float *, float *, float, float, float, float);
int SPW_ClipZ (float *, float *, float, float);

void SPW_Filter (float [7]);
void SPW_Scale (float [7]);
void SPW_MultQuaternion (float [4], float [4], float [4]);
void SPW_MatrixToEulerA (float [3], Matrix);
void SPW_EulerAToMatrix (Matrix, float [3]);
void SPW_MatrixToQuaternion (float [4], Matrix);
void SPW_QuaternionToMatrix (Matrix, float [4]);
void SPW_InvertEulerA (float [3], float [3]);
void SPW_InvertQuaternion (float [4], float [4]);
void SPW_AddMatrices (Matrix,Matrix, Matrix);
void SPW_SubtractMatrices (Matrix, Matrix, Matrix);
void SPW_DifferenceMatrix (Matrix, Matrix, Matrix);
void SPW_GetRotationMatrix (Matrix, Matrix);
void SPW_SetRotationPartOfMatrix (Matrix, Matrix);
void SPW_SetTranslationPartOfMatrix (Matrix, float [3]);
void SPW_GetTranslationVector (float [3], Matrix);
void SPW_MakeXRotationMatrix (Matrix, float);
void SPW_MakeYRotationMatrix (Matrix, float);
void SPW_MakeZRotationMatrix (Matrix, float);
void SPW_MakeTranslationMatrix (Matrix, float [3]);
void SPW_MakeScaleMatrix (Matrix, float, float, float);
void SPW_GridSnapMatrix (Matrix, Matrix);
void SPW_NormalizeQuaternion (float [4], float [4]);
void SPW_ScaleVector (float [3], float [3], float);
void SPW_HighPercentagePassFilter (float *, int , float);
void SPW_HighNPassFilter (float *, int, int);
void SPW_PerspectiveToMatrix (Matrix, short, float, float, float);
void SPW_OrthoToMatrix (Matrix, float, float, float, float, float, float);

/* Every viewing mode function takes an address of a ptr to one of these 
   spw_State_st structures.  Initially, the ptr is NULL, and is assigned 
   by the first viewing function that is called with it.  
   Unfortunately, there just is not any way around saving a few things.
*/
struct spw_State_st
   {
   float last_orbit_pt[3],
         last_orbit_dir[3],
         last_orbit_up[3];
   };

/* PRIVATE MACROS, DEFINES */

#define CHECKSTATE(p) /* Make sure the state is allocated and initialized */ \
   if (!*p) {\
      *p = (SPW_State_t) malloc (sizeof(struct spw_State_st));\
      (*p)->last_orbit_pt[0] = (*p)->last_orbit_pt[1] = (*p)->last_orbit_pt[2] = 0.0f;\
      (*p)->last_orbit_dir[0] = (*p)->last_orbit_dir[1] = 0.0f;\
      (*p)->last_orbit_dir[2] = -1.0f;\
      (*p)->last_orbit_up[0] = (*p)->last_orbit_up[2] = 0.0f;\
      (*p)->last_orbit_up[1] = 1.0f;\
      }
#define PSTATE (*pstate)

#ifdef __cplusplus
}
#endif

#endif /* SPW_UTIL_H */






