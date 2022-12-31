using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRP_guo4_character : MonoBehaviour
{
    public float shakeValue = 5f;
    public float shaking = 0f;
    private Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(originalPos.x + UnityEngine.Random.Range(-shakeValue, shakeValue) * shaking, originalPos.y + UnityEngine.Random.Range(-shakeValue, shakeValue) * shaking, originalPos.z + UnityEngine.Random.Range(-shakeValue, shakeValue) * shaking);
        
        if(shaking <= 0) 
        {
            shaking = 0;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else 
        {
            shaking -= 0.05f;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }










    Vector2 Math_Path_Circle(Vector2 Origin, float Radius, float Degree)
    {   //角度是以(1, 0)方向为0度的
        Degree = Degree % 360f;//使角度始终限制在360度内
        return new Vector2(Radius * Mathf.Cos(Degree * (Mathf.PI / 180f)), Radius * Mathf.Sin(Degree * (Mathf.PI / 180f))) + Origin;
    }
    float Math_GetRotationBetweenVector_360(Vector2 A, Vector2 B)//能够将0~180度映射至0~360度的版本
    {   //叉乘<0时朝上，叉乘>0时朝下
        if(Math_Cross(Math_Normalize(Math_Vector2to3(A)), Math_Normalize(Math_Vector2to3(B))).z > 0) 
        {
            return 360 - Math_GetRotationBetweenVector(A, B);
        }
        else return Math_GetRotationBetweenVector(A, B);
    }
    float Math_GetRotationBetweenVector(Vector2 A, Vector2 B)//Acos只能输出0~180度
    {   //Acos返回的是弧度，后面乘数转成角度
        return (float)Mathf.Acos(Mathf.Clamp(Math_Dot(Math_Normalize(A), Math_Normalize(B)), -1f, 1f)) * (180f / Mathf.PI);
    }
    float Math_GetVectorLength(Vector2 VECTOR, bool abs = false)
    {
        if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
        else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
    }
    float Math_GetVectorLength(Vector3 VECTOR, bool useVec2 = false, bool abs = false)
    {
        if (useVec2 == false)
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2) + Mathf.Pow(VECTOR.z, 2));
        }
        else
        {
            if(abs == true) return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2)));
            else return Mathf.Sqrt(Mathf.Pow(VECTOR.x, 2) + Mathf.Pow(VECTOR.y, 2));
        }
    }
    float Math_Dot(Vector2 A, Vector2 B)
    {
        return A.x * B.x + A.y * B.y;
    }
    float Math_Cross(Vector2 A, Vector2 B)
    {
        return (A.x * B.y - A.y * B.x);
    }
    Vector3 Math_Cross(Vector3 A, Vector3 B)
    {
        return new Vector3(A.y * B.z - A.z * B.y, A.z * B.x - B.z * A.x, A.x * B.y - A.y * B.x);
    }
    Vector3 Math_Vector2to3(Vector2 A)
    {
        return new Vector3(A.x, A.y, 0f);
    }
    Vector2 Math_Vector3to2(Vector3 A)
    {
        return new Vector2(A.x, A.y);
    }
    Vector3 Math_Normalize(Vector3 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        if(A.z == 0) A.z = Mathf.Epsilon;
        return new Vector3(A.x / Math_GetVectorLength(A, false, true), A.y / Math_GetVectorLength(A, false, true), A.z / Math_GetVectorLength(A, false, true));
    }
    Vector2 Math_Normalize(Vector2 A)
    {
        if(A.x == 0) A.x = Mathf.Epsilon;
        if(A.y == 0) A.y = Mathf.Epsilon;
        return new Vector2(A.x / Math_GetVectorLength(A, true), A.y / Math_GetVectorLength(A, true));
    }
    Vector3 Math_LerpVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C), Mathf.Lerp(A.z, B.z, C));
    }
    Vector2 Math_LerpVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector3(Mathf.Lerp(A.x, B.x, C), Mathf.Lerp(A.y, B.y, C));
    }
    Vector3 Math_MoveVector(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C), Mathf.MoveTowards(A.z, B.z, C));
    }
    Vector2 Math_MoveVector(Vector2 A, Vector2 B, float C)
    {
        return new Vector2(Mathf.MoveTowards(A.x, B.x, C), Mathf.MoveTowards(A.y, B.y, C));
    }
    Vector3 Math_GradVector(Vector3 A, Vector3 B, float MoveSpeed, float LerpSpeed)
    {
        return Math_MoveVector(A, Math_LerpVector(A, B, LerpSpeed), MoveSpeed);
    }
    Vector2 Math_GradVector(Vector2 A, Vector2 B, float MoveSpeed, float LerpSpeed)
    {
        return Math_MoveVector(A, Math_LerpVector(A, B, LerpSpeed), MoveSpeed);
    }

    public T Selector<T>(T A, T B, bool isA)
    {
        if(isA) return A;
        else return B;
    }
}
