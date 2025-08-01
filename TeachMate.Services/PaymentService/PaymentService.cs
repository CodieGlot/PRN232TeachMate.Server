﻿using Microsoft.EntityFrameworkCore;
using TeachMate.Domain;
using TeachMate.Domain.Models.Payment;

namespace TeachMate.Services;
public class PaymentService : IPaymentService
{
    private readonly IZaloPayService _zaloPayService;
    private readonly IMomoService _momoService;
    private readonly IVnPayService _vnPayService;
    private readonly DataContext _context;

    public PaymentService(IZaloPayService zaloPayService, IMomoService momoService, IVnPayService vnPayService, DataContext context)
    {
        _zaloPayService = zaloPayService;
        _momoService = momoService;
        _vnPayService = vnPayService;
        _context = context;
    }
    public async Task<OrderUrlResponseDto> CreateOrderUrl(double amount, PaymentProviderType type, string tick)
    {

        return type switch
        {
            PaymentProviderType.ZaloPay => await _zaloPayService.CreateZaloPayOrder(amount),
            PaymentProviderType.Momo => await _momoService.CreateMomoOrder(amount),
            PaymentProviderType.VnPay => _vnPayService.CreateVnPayOrder(amount, tick),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<LearningModulePaymentOrder> CreatePaymentOrder(CreateOrderPaymentDto dto)
    {
        var amount = await _context.LearningModules
            .Where(p => p.Id == dto.LearningModuleId)
            .Select(p => p.Price)
            .FirstOrDefaultAsync();
        var learningModule = await _context.LearningModules
            .Where(p => p.Id == dto.LearningModuleId)
            .FirstOrDefaultAsync();
        var learner = await _context.Learners
            .SingleOrDefaultAsync(l => l.Id == dto.LearnerID);

        if (learner == null)
        {
            throw new Exception("Invalid LearnerId");
        }

        var order = new LearningModulePaymentOrder
        {
            Learner = learner,
            LearningModule = learningModule,
            PaymentAmount = amount,
            CreatedAt = DateTime.UtcNow,
            HasClaimed = false,
            PaymentStatus = PaymentStatus.Pending,
        };

        await _context.LearningModulePaymentOrders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<ResponseDto> PayForClass(int OrderID)
    {

        var paid = await _context.LearningModulePaymentOrders.Where(p => p.Id == OrderID).FirstOrDefaultAsync();
        if (paid == null)
        {
            throw new BadRequestException("Not found your order");
        }
        paid.PaymentStatus = PaymentStatus.Paid;
        _context.LearningModulePaymentOrders.Update(paid);
        await _context.SaveChangesAsync();
        return new ResponseDto("paid success");
    }

    public async Task<List<LearningModulePaymentOrder>> GetAllPaymentOrder(Guid LearnerId)
    {
        var ListPaymentOrder = await _context.LearningModulePaymentOrders.Where(p => p.LearnerId == LearnerId).Select(p =>
        new LearningModulePaymentOrder
        {
            PaymentAmount = p.PaymentAmount,
            CreatedAt = p.CreatedAt,
            Id = p.Id,
            LearnerId = p.LearnerId,
            LearningModuleId = p.LearningModuleId,
            HasClaimed = p.HasClaimed,
            PaymentStatus = PaymentStatus.Paid,
        }).ToListAsync();
        return ListPaymentOrder;
    }
    public async Task<List<LearningModulePaymentOrder>> GetAllPaymentOrderByModuleID(int moduleID)
    {
        var ListPaymentOrder = await _context.LearningModulePaymentOrders.Where(p => p.LearningModuleId == moduleID).Select(p =>
        new LearningModulePaymentOrder
        {
            PaymentAmount = p.PaymentAmount,
            CreatedAt = p.CreatedAt,
            Id = p.Id,
            LearnerId = p.LearnerId,
            LearningModuleId = p.LearningModuleId,
            HasClaimed = p.HasClaimed,
            PaymentStatus = PaymentStatus.Paid,
        }).ToListAsync();
        if (ListPaymentOrder == null)
        {
            throw new BadRequestException("No payment order of the class");
        }
        return ListPaymentOrder;
    }
    public async Task<LearningModulePaymentOrder> GetAllPaymentOrderUnpaidByModuleIdByLearner(int moduleID, Guid LearnerId)
    {
        var ListPaymentOrder = await _context.LearningModulePaymentOrders.Where(p => p.LearningModuleId == moduleID && p.LearnerId == LearnerId && p.PaymentStatus == PaymentStatus.Pending).FirstOrDefaultAsync();
        if (ListPaymentOrder == null)
        {
            throw new Exception("Unpaid Order not found");
        }
        return ListPaymentOrder;
    }
    public async Task<List<LearningModulePaymentOrder>> GetAllPaymentOrderByModuleIdByLearner(int moduleID, Guid LearnerId)
    {
        var ListPaymentOrder = await _context.LearningModulePaymentOrders.Where(p => p.LearningModuleId == moduleID && p.LearnerId == LearnerId).ToListAsync();
        if (ListPaymentOrder == null)
        {
            throw new Exception("Payment Order not found");
        }
        return ListPaymentOrder;
    }




    public async Task<LearningModule> SetPriceForLearningModule(SetPriceForLearningModuleDto dto)
    {
        var learningModule = await _context.LearningModules
        .Where(lm => lm.Id == dto.LearningModuleId)
        .FirstOrDefaultAsync();

        if (learningModule == null)
        {
            throw new ArgumentException("Learning module not found.");
        }

        if (learningModule.ModuleType == ModuleType.Custom)
        {
            learningModule.PaymentType = PaymentType.Session;

        }
        else if (learningModule.ModuleType == ModuleType.Weekly)
        {
            if (dto.PaymentType == PaymentType.Session || dto.PaymentType == PaymentType.Weekly)
            {
                learningModule.PaymentType = dto.PaymentType;
            }
            else
            {
                throw new ArgumentException("Invalid payment type for weekly module. Must be either Session or Weekly.");
            }
        }
        else
            learningModule.PaymentType = dto.PaymentType;

        learningModule.Price = dto.Price;
        _context.Update(learningModule);
        await _context.SaveChangesAsync();

        return learningModule;
    }

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto)
    {
        var existingTransactions = await _context.Transactions
                .Where(t => t.LearningModulePaymentOrderId == dto.LearningModulePaymentOrderId)
                .ToListAsync();

        _context.Transactions.RemoveRange(existingTransactions);
        var learningModulePaymentOrder = await _context.LearningModulePaymentOrders.FirstOrDefaultAsync(x => x.Id == dto.LearningModulePaymentOrderId);

        Transaction t = new Transaction()
        {
            Amount = dto.Amount,
            LearningModulePaymentOrder = learningModulePaymentOrder,
            PaymentGateway = dto.PaymentGateway,
            TxnRef = DateTime.UtcNow.Ticks.ToString()
        };
        _context.Transactions.Add(t);
        await _context.SaveChangesAsync();
        return t;
    }

    public async Task<Transaction> UpdateTransactionAsync(UpdateTransactionDto dto)
    {
        string dateFormat = "yyyyMMddHHmmss";
        DateTime datePay = DateTime.ParseExact(dto.PayDate, dateFormat, null);
        var existingTransaction = _context.Transactions.Include(x => x.LearningModulePaymentOrder).FirstOrDefault(x => x.TxnRef.Equals(dto.TxnRef));
        if (existingTransaction != null)
        {
            existingTransaction.Amount = dto.Amount;
            existingTransaction.TxnRef = dto.TxnRef;
            existingTransaction.TransactionNo = dto.TransactionNo;
            existingTransaction.TransactionStatus = dto.TransactionStatus;
            existingTransaction.OrderInfo = dto.OrderInfo;
            existingTransaction.BankCode = dto.BankCode;
            existingTransaction.CardType = dto.CardType;
            existingTransaction.PayDate = datePay;
            existingTransaction.ResponseCode = dto.ResponseCode;
            existingTransaction.SecureHash = dto.SecureHash;
            existingTransaction.TmnCode = dto.TmnCode;
            if (dto.TransactionStatus.Equals("00")) { await PayForClass(existingTransaction.LearningModulePaymentOrderId); }
            _context.Update(existingTransaction);

            await _context.SaveChangesAsync();

            return existingTransaction;
        }

        return null!;
    }
    // have paid, have enroll
    public async Task<bool> CheckPermissionToViewLearningModule(Guid learnerId, int partId)
    {
        bool hasPaid = await _context.LearningModulePaymentOrders
                           .AnyAsync(p => p.LearnerId == learnerId &&
                                          p.LearningModuleId == partId &&
                                          p.PaymentStatus == PaymentStatus.Paid) &&
                                      !(await _context.LearningModulePaymentOrders
                           .AnyAsync(p => p.LearnerId == learnerId &&
                                          p.LearningModuleId == partId &&
                                          p.PaymentStatus == PaymentStatus.Pending));

        return hasPaid;
    }

    public async Task<AccountInformation> AddAccountInformation(AddAccountInformationDto dto, Guid tutorId)
    {
        var accountInfo = new AccountInformation()
        {
            AccountNumber = dto.AccountNumber,
            BankCode = dto.BankCode,
            FullName = dto.FullName,
            TaxCode = dto.TaxCode,
            TutorId = tutorId
        };
        await _context.AccountInformations.AddAsync(accountInfo);
        await _context.SaveChangesAsync();
        return accountInfo;
    }

    public async Task<AccountInformation> GetAccountInformationByTutorId(Guid tutorId)
    {
        var accountInfo = await _context.AccountInformations.FirstOrDefaultAsync(a => a.TutorId == tutorId);
        return accountInfo!;
    }

    public async Task<bool> ExistedAccountInformationByTutorId(Guid tutorId)
    {
        var accountInfo = await _context.AccountInformations.AnyAsync(a => a.TutorId == tutorId);
        return accountInfo;
    }
}

