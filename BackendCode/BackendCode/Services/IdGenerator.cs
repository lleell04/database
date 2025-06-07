using Yitter.IdGenerator;
namespace BackendCode.Services
{
    public class IdGenerator
    {
       /* private readonly string _filePath;
        private readonly object _lock = new object();
        private int _currentId;
        private int _idLength;*/

        public IdGenerator(/*string filePath,int idLength=8*/)
        {
            /*_filePath = filePath;
            _currentId = ReadCurrentIdFromFile();
            _idLength = idLength;//默认生成八位长度ID*/
        }

        public string GetNextId()
        {
            /*lock (_lock)
            {
                int newId = _currentId++;
                WriteCurrentIdToFile(_currentId);
                return newId.ToString($"D{_idLength}"); // 将newId格式化为固定位数的字符串
            }*/
            string nextId=YitIdHelper.NextId().ToString();
            return nextId;
        }

        /*private int ReadCurrentIdFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return 0; // 初始ID值为0
            }

            string idStr = File.ReadAllText(_filePath);
            if (int.TryParse(idStr, out int id))
            {
                return id;
            }
            else
            {
                throw new InvalidOperationException("无法从文件中读取有效的ID值");
            }
        }*/

        /*private void WriteCurrentIdToFile(int id)
        {
            File.WriteAllText(_filePath, id.ToString());
        }*/
    }
}


// 使用示例
/*public class Program
{
    public static void Main()
    {
        string filePath = "current_id.txt";
        UniqueIdGenerator idGenerator = new UniqueIdGenerator(filePath);

        // 生成一些ID
        for (int i = 0; i < 10; i++)
        {
            int newId = idGenerator.GetNextId();
            Console.WriteLine("生成的ID: " + newId);
        }
    }
}*/
