using System;

public static class Extension_Array
{

    /// <summary>
    /// Get the first id with a null value of the array
    /// </summary>
    /// <returns>The first null id found
    /// <br>-1 if there is no empty id</br>
    /// </returns>
    public static int GetFirstNullId(this Array array)
    {
        int arrayLenght = array.Length;

        for (int id = 0; id < arrayLenght; id++)
        {
            object current = array.GetValue(id);

            if (current == null)
                return id;
        }

        return -1;
    }
}
