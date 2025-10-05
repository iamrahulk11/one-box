using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace domain.Wrapper
{
    #region ---------------- FOR USERNAME AVAILABILITY --------------
    public class BloomFilterWrapper
    {
        private BitArray _bitArray;
        private readonly int _bitSize;
        private readonly int _hashCount;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a Bloom filter with the given size (in bits) and number of hash functions.
        /// </summary>
        public BloomFilterWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _bitSize = Convert.ToInt32(_configuration["bloom_filter:bitsize"]);
            _hashCount = Convert.ToInt32(_configuration["bloom_filter:hashcount"]);
        }

        /// <summary>
        /// Checks if the value might exist in the Bloom filter.
        /// Returns false if the value is definitely not present.
        /// </summary>
        public bool Contains(string item)
        {
            string baseFilePath = _configuration["bloom_filter:bit_array_data_filepath"].ToString();

            if (!File.Exists(baseFilePath))
                throw new FileNotFoundException("Bloom filter file not found.", baseFilePath);

            string base64 = File.ReadAllText(baseFilePath).Trim();
            byte[] bytes = Convert.FromBase64String(base64);
            _bitArray = new BitArray(bytes) { Length = _bitSize };

            if (string.IsNullOrWhiteSpace(item))
                return false;

            foreach (int position in GetHashPositions(item))
            {
                if (!_bitArray[position])
                    return false;
            }

            return true; // Might exist (possible false positive)
        }

        /// <summary>
        /// Uses double hashing to generate multiple independent hash positions.
        /// </summary>
        private int[] GetHashPositions(string item)
        {
            byte[] itemBytes = Encoding.UTF8.GetBytes(item);

            // First hash: SHA256
            byte[] hash1Bytes = SHA256.HashData(itemBytes);
            int hash1 = BitConverter.ToInt32(hash1Bytes, 0) & 0x7FFFFFFF;

            // Second hash: use next 4 bytes from SHA256
            int hash2 = BitConverter.ToInt32(hash1Bytes, 4) & 0x7FFFFFFF;

            int[] positions = new int[_hashCount];

            for (int i = 0; i < _hashCount; i++)
            {
                long combinedHash = (long)hash1 + i * (long)hash2;
                positions[i] = (int)(combinedHash % _bitSize);
            }

            return positions;
        }
    }

    #endregion

    #region ------------------ FOR DATA POPULATE ---------------------
    public class BloomFilterPopulator
    {
        private readonly int _bitSize;
        private readonly int _hashCount;
        private readonly IConfiguration _configuration;
        private string _filePath;
        public BloomFilterPopulator(IConfiguration configuration)
        {
            _configuration = configuration;
            _bitSize = Convert.ToInt32(_configuration["bloom_filter:bitsize"]);
            _hashCount = Convert.ToInt32(_configuration["bloom_filter:hashcount"]);
            _filePath = Convert.ToString(_configuration["bloom_filter:bit_array_data_filepath"]);
        }

        public async Task Add(string item, BitArray bit_array)
        {
            foreach (int position in GetHashPositions(item))
            {
                bit_array[position] = true;
            }

            await File.WriteAllTextAsync(_filePath, Serialize(bit_array));
        }

        public async Task PopulateAsync(string usernames, bool appendUser)
        {
            bool isFileExhausted = false;
            BitArray _bitArray = new BitArray(_bitSize);

            if (!File.Exists(_filePath))
            {
                _filePath = Path.Combine(AppContext.BaseDirectory, "/Document/Bloom-Filter/username_filter_temp.txt");
                isFileExhausted = true;
            }

            if (string.IsNullOrWhiteSpace(File.ReadAllText(_filePath)))
            {
                _bitArray = new BitArray(_bitSize);
                isFileExhausted = true;
            }
            if (appendUser && !isFileExhausted)
                _bitArray = Deserialize(_bitArray);

            await Add(usernames, _bitArray);
        }

        public string Serialize(BitArray bit_array)
        {
            int numBytes = (bit_array.Length + 7) / 8;
            byte[] bytes = new byte[numBytes];
            bit_array.CopyTo(bytes, 0);
            return Convert.ToBase64String(bytes);
        }

        private BitArray Deserialize(BitArray bit_array)
        {
            string base64 = File.ReadAllText(_filePath); 
            byte[] bytes = Convert.FromBase64String(base64); 
            return new BitArray(bytes) { Length = _bitSize };
        }

        private int[] GetHashPositions(string item)
        {
            byte[] itemBytes = Encoding.UTF8.GetBytes(item);
            byte[] hashBytes = SHA256.HashData(itemBytes);

            int hash1 = BitConverter.ToInt32(hashBytes, 0) & 0x7FFFFFFF;
            int hash2 = BitConverter.ToInt32(hashBytes, 4) & 0x7FFFFFFF;

            int[] positions = new int[_hashCount];

            for (int i = 0; i < _hashCount; i++)
            {
                long combined = (long)hash1 + i * (long)hash2;
                positions[i] = (int)(combined % _bitSize);
            }

            return positions;
        }
    }

    #endregion -------------------------------------------------------
}