using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Camera
    {
        public   float mAngleX = 0;
        public float prevAngleX = 0;
        float mAngleY = 0;
        vec3 mDirection;
        vec3 mPosition;
        public vec3 LastPos;
        public vec3 lastPosBeforeCamCollision;
        vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;

        public bool getPrevious = true;
        public bool collision;
        float prevDist;

        public Camera()
        {
            prevDist = 0;
            collision = false;
            Reset(0, 0, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0f, 1000);
        }

        public vec3 GetLookDirection()
        {
            return mDirection;
        }
        public vec3 getLastPos()
        {
            return LastPos;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }

        public void undoMove()
        {
            vec3 center = lastPosBeforeCamCollision + mDirection;
            Reset(lastPosBeforeCamCollision.x, lastPosBeforeCamCollision.y, lastPosBeforeCamCollision.z, center.x, center.y, center.z, mUp.x, mUp.y, mUp.z);
            collision = false;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            vec3 centerPos = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);

            mPosition = eyePos;
            LastPos = mPosition;
            lastPosBeforeCamCollision = mPosition;
            mDirection = centerPos - mPosition;
            mRight = glm.cross(mDirection, upVec);
            mUp = upVec;
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);

            mViewMatrix = glm.lookAt(mPosition, centerPos, mUp);
        }

        public void UpdateViewMatrix()
        {
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , 0
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            mUp = glm.cross(mRight, mDirection);
            if(mPosition.x >=99)
            {
                mPosition.x = 99;
            }
            if (mPosition.x <= -99)
            {
                mPosition.x = -99;
            }


            if (mPosition.z >= 99)
            {
                mPosition.z = 99;
            }
            if (mPosition.z <= -99)
            {
                mPosition.z = -99;
            }


            vec3 center = mPosition + mDirection;
            mViewMatrix = glm.lookAt(mPosition, center, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }


        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Pitch(float angleDegrees)
        {
            mAngleY += angleDegrees;
        }

        public void Walk(float dist)
        {
            if (collision)
            {
                mPosition = lastPosBeforeCamCollision;
            }
            if(!collision )
            {
                prevDist = dist;
                lastPosBeforeCamCollision = mPosition;
                //LastPos = mPosition;
                mPosition += dist * mDirection;
            }
            
        }
        public void Strafe(float dist)
        {
            if (collision)
            {
                mPosition = lastPosBeforeCamCollision;
            }
            if ( !collision)
            {
                prevDist = dist;
                lastPosBeforeCamCollision = mPosition;
                //LastPos = mPosition;
                mPosition += dist * mRight;
            }
        }
        public void Fly(float dist)
        {
            mPosition += dist * mUp;
        }
    }
}
