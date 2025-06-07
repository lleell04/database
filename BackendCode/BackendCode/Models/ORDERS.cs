using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System;

namespace BackendCode.Models
{
    public class ORDERS
    {
        public string ORDER_ID { get; set; }

        public string PRODUCT_ID { get; set; }

        public decimal TOTAL_PAY { get; set; }
        public decimal ACTUAL_PAY { get; set; }

        public string ORDER_STATUS { get; set; }
        public DateTime CREATE_TIME { get; set; }
        public DateTime? RECIEVING_TIME { get; set; }

        public string? DELIVERY_NUMBER { get; set; }

        public decimal SCORE { get; set; } = 0;

        public string? REMARK { get; set; }

        public int  BONUS_CREDITS { get; set; }

        public bool? RETURN_OR_NOT { get; set; }

        public string BUYER_ACCOUNT_ID { get; set; }

        public string STORE_ACCOUNT_ID { get; set; }
        public string ?DELIVERY_ADDRESS { get; set; }
        public string ?USERNAME { get; set; }

        // 导航属性
        public PRODUCT PRODUCT { get; set; }

        public BUYER BUYER { get; set; }
        public STORE STORE { get; set; }
    }

}
