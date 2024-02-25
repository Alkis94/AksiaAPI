using AksiaAPI.Models.Business;
using AksiaAPI.Models.Entities;
using AksiaAPI.Repositories.Interfaces;
using AksiaAPI.Services.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace AksiaAPI.Services
{
    public class TransactionService : ITransactionService
    {
        #region Constructor and Fields
        private readonly IUnitOfWork _unitOfWork;

        private readonly string[] _validFileExtensions = new [] { ".png", ".mp3", ".tiff", ".xls", ".pdf" };

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        #endregion

        #region Public Methods
        public async Task<Transaction> Get(Guid id)
        {
            if (id == default)
            {
                throw new ArgumentException(nameof(id));
            }

            var transaction = await _unitOfWork.TransactionRepository.FindAsync(id);

            if (transaction == null)
            {
                throw new BusinessException($"Transaction with id {id} was not found.");
            }

            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetPagedAsync(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            return await _unitOfWork.TransactionRepository.GetPagedAsync(page);
        }

        public async Task<Guid> Insert(TransactionInsertOrUpdate transactionInsert)
        {
            if (transactionInsert == null)
            {
                throw new ArgumentNullException(nameof(transactionInsert));
            }

            var validationResult = IsValidTransaction(transactionInsert);

            if (!validationResult.IsValid)
            {
                throw new BusinessException(validationResult.ErrorMessage);
            }

            var transaction = new Transaction
            {
                Id = new Guid(),
                ApplicationName = transactionInsert.ApplicationName!,
                Email = transactionInsert.Email!,
                Filename = transactionInsert.Filename,
                Url = !string.IsNullOrWhiteSpace(transactionInsert.Url) ? new Uri(transactionInsert.Url) : null,
                Inception = (DateTime)transactionInsert.Inception!,
                Amount = double.Parse(transactionInsert.Amount[1..], System.Globalization.CultureInfo.InvariantCulture),
                Allocation = transactionInsert.Allocation
            };

            _unitOfWork.TransactionRepository.Add(transaction);
            await _unitOfWork.SaveChangesAsync();

            return transaction.Id;
        }

        public async Task Delete(Guid id)
        {
            if (id == default)
            {
                throw new ArgumentException(nameof(id));
            }

            var transaction = await _unitOfWork.TransactionRepository.FindAsync(id);

            if (transaction == null)
            {
                throw new BusinessException($"Transaction with id {id} was not found.");
            }

            _unitOfWork.TransactionRepository.Delete(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> ParseCSV (IFormFile file)
        {
            var transactionsToUpdate = new List<Transaction>();
            var transactionsToCreate = new List<Transaction>();
            var invalidTransactions = new List<(int, string)>();
            var numberOfTransactions = 0;

            using (var csvParser = new TextFieldParser(file.OpenReadStream()))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    numberOfTransactions++;

                    var fields = csvParser.ReadFields();

                    if (fields == null)
                    {
                        continue;
                    }

                    var transactionCreateOrUpdate = new TransactionInsertOrUpdate
                    {
                        Id = fields[0],
                        ApplicationName = fields[1],
                        Email = fields[2],
                        Filename = fields[3],
                        Url = fields[4],
                        Inception = DateTime.ParseExact(fields[5], "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Amount = fields[6],
                        Allocation = string.IsNullOrWhiteSpace(fields[7]) ? null : decimal.Parse(fields[7])
                    };

                    var validationResult = IsValidTransaction(transactionCreateOrUpdate);

                    if (!validationResult.IsValid)
                    {
                        invalidTransactions.Add((numberOfTransactions, validationResult.ErrorMessage));
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(transactionCreateOrUpdate.Id))
                    {
                        var transaction = new Transaction
                        {
                            Id = new Guid(),
                            ApplicationName = transactionCreateOrUpdate.ApplicationName,
                            Email = transactionCreateOrUpdate.Email,
                            Filename = transactionCreateOrUpdate.Filename,
                            Url = !string.IsNullOrWhiteSpace(transactionCreateOrUpdate.Url) ? new Uri(transactionCreateOrUpdate.Url) : null,
                            Inception = (DateTime)transactionCreateOrUpdate.Inception,
                            Amount = double.Parse(transactionCreateOrUpdate.Amount[1..], System.Globalization.CultureInfo.InvariantCulture),
                            Allocation = transactionCreateOrUpdate.Allocation
                        };

                        transactionsToCreate.Add(transaction);
                    }
                    else
                    {
                        var transactionId = Guid.Parse(transactionCreateOrUpdate.Id);
                        var transaction = await _unitOfWork.TransactionRepository.FindAsync(transactionId);

                        if (transaction == null)
                        {
                            transaction = new Transaction
                            {
                                Id = transactionId,
                                ApplicationName = transactionCreateOrUpdate.ApplicationName,
                                Email = transactionCreateOrUpdate.Email,
                                Filename = transactionCreateOrUpdate.Filename,
                                Url = !string.IsNullOrWhiteSpace(transactionCreateOrUpdate.Url) ? new Uri(transactionCreateOrUpdate.Url) : null,
                                Inception = (DateTime)transactionCreateOrUpdate.Inception,
                                Amount = double.Parse(transactionCreateOrUpdate.Amount[1..], System.Globalization.CultureInfo.InvariantCulture),
                                Allocation = transactionCreateOrUpdate.Allocation
                            };

                            transactionsToCreate.Add(transaction);
                        }
                        else
                        {
                            transaction.ApplicationName = transactionCreateOrUpdate.ApplicationName;
                            transaction.Email = transactionCreateOrUpdate.Email;
                            transaction.Filename = transactionCreateOrUpdate.Filename;
                            transaction.Url = !string.IsNullOrWhiteSpace(transactionCreateOrUpdate.Url) ? new Uri(transactionCreateOrUpdate.Url) : null;
                            transaction.Inception = (DateTime)transactionCreateOrUpdate.Inception;
                            transaction.Amount = double.Parse(transactionCreateOrUpdate.Amount[1..], System.Globalization.CultureInfo.InvariantCulture);
                            transaction.Allocation = transactionCreateOrUpdate.Allocation;

                            transactionsToUpdate.Add(transaction);
                        }
                    }
                }
            }

            if (transactionsToCreate.Count > 0)
            {
                _unitOfWork.TransactionRepository.AddRange(transactionsToCreate);
            }

            if (transactionsToUpdate.Count > 0)
            {
                _unitOfWork.TransactionRepository.UpdateRange(transactionsToUpdate);
            }

            await _unitOfWork.SaveChangesAsync();

            var invalidTransactionsString = "";
            foreach(var invalidTransaction in invalidTransactions)
            {
                invalidTransactionsString += $"Transaction Line: {invalidTransaction.Item1} Error: {invalidTransaction.Item2} \n";
            }

            return $"Transactions Created: {transactionsToCreate.Count}, Transactions Updated: {transactionsToUpdate.Count} \n" +
                $"Invalid Transactions: {numberOfTransactions - (transactionsToCreate.Count + transactionsToUpdate.Count)} \n" + 
                invalidTransactionsString;
        }

        #endregion

        #region Private Methods

        private ValidationResult IsValidTransaction(TransactionInsertOrUpdate transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (string.IsNullOrWhiteSpace(transaction.ApplicationName))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Application Name is mandatory."
                };
            }

            if (transaction.ApplicationName.Length > 200)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Application Name cannot exceed 200 characters."
                };
            }

            if (string.IsNullOrWhiteSpace(transaction.Email))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Email is mandatory."
                };
            }

            if (transaction.Email.Length > 200)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Email cannot exceed 200 characters."
                };
            }

            if (!string.IsNullOrWhiteSpace(transaction.Filename))
            {
                if (transaction.Filename.Length > 300)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "File name cannot exceed 300 characters."
                    };
                }

                var extension = Path.GetExtension(transaction.Filename);

                if (!_validFileExtensions.Contains(extension))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Valid file name extensions are: {string.Join(", ", _validFileExtensions)}"
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(transaction.Url))
            {
                if (!Uri.IsWellFormedUriString(transaction.Url, UriKind.Absolute))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Url is invalid."
                    };
                }
            }

            if (transaction.Inception == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Inception is mandatory."
                };
            }

            if (transaction.Inception > DateTime.UtcNow)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Inception should be in the past."
                };
            }

            if (string.IsNullOrWhiteSpace(transaction.Amount))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Amount is mandatory."
                };
            }

            if (!string.IsNullOrWhiteSpace(transaction.Amount))
            {
                var firstCharacter = transaction.Amount[0].ToString();

                if (int.TryParse(firstCharacter, out _))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Amount should be begin with a currency identifier. Ex: $56.32"
                    };
                }

                var amountWithoutCurrency = transaction.Amount[1..];

                if (!double.TryParse(amountWithoutCurrency, out _))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Amount should be a number with a currency identifier as the first character only. Ex: $56.32"
                    };
                }
            }

            if (transaction.Allocation != null)
            {
                if (transaction.Allocation < 0 || transaction.Allocation > 100)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Allocation should be between 0 - 100."
                    };
                }
            }

            return new ValidationResult
            {
                IsValid = true,
                ErrorMessage = ""
            };
        }

        #endregion
    }
}
