using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaTalkEspresso
{
    class Util
    {
        // 파일 시스템이 대소문자 구분하는지 기억. (구현 안 됨)
        private static bool isFsCaseSensitive = false;

        //public static string toLowerComplyingFs(string filepath)
        //{
        //    if ( ! isFsCaseSensitive && filepath != null)
        //    {
        //        filepath = filepath.ToLower();
        //    }

        //    return filepath;
        //}

        public static bool checkForFlag(int whole, int specificFlag)
        {
            // 플래그 설정 여부만 boolean으로 반환
            if ((whole & specificFlag) == specificFlag)
            {
                // 플래그 설정됨
                return true;
            }

            //플래그 설정 안 되거나 부분만 됨
            return false;
        }

        public static int setFlag(int whole, int specificFlag)
        {
            // 플래그와 OR 연산 진행
            return whole | specificFlag;
        }

        public static int unsetFlag(int whole, int specificFlag)
        {
            // 플래그와 OR 연산 진행한 뒤 빼기
            whole = whole | specificFlag;

            return whole - specificFlag;
        }

        public static IntPtr bitAndIntPtr(IntPtr other, IntPtr another)
        {
            // IntPtr 끼리는 비트연산자 사용불가.
            // 따라서 Int64로 변환 후 비트 연산자 사용한 결과를 IntPtr로 반환 
            return new IntPtr( other.ToInt64() & another.ToInt64() );
        }

        public static IntPtr Int64ToIntPtr(Int64 longVal)
        {
            return new IntPtr(longVal);
        }

        public static IntPtr Int32ToIntPtr(Int32 intVal)
        {
            return new IntPtr(intVal);
        }
    }
}
