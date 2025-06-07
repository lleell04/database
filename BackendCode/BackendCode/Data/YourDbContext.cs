using Microsoft.EntityFrameworkCore;
using BackendCode.Models;
using System.Reflection.Metadata;

namespace BackendCode.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options)
           : base(options)
        {
        }
        public virtual DbSet<ACCOUNT> ACCOUNTS { get; set; }
        public virtual DbSet<ADMINISTRATOR> ADMINISTRATORS { get; set; }
        public virtual DbSet<BUYER> BUYERS { get; set; }
        public virtual DbSet<BUYER_PRODUCT_BOOKMARK> BUYER_PRODUCT_BOOKMARKS { get; set; }
        public virtual DbSet<BUYER_STORE_BOOKMARK> BUYER_STORE_BOOKMARKS { get; set; }
        public virtual DbSet<COMMENT_POST> COMMENT_POSTS { get; set; }
        public virtual DbSet<COMMENT_COMMENT> COMMENT_COMMENTS { get; set; }
        public virtual DbSet<COMPLAIN_POST> COMPLAIN_POSTS { get; set; }
        public virtual DbSet<COMPLAIN_COMMENT> COMPLAIN_COMMENTS { get; set; }
        public virtual DbSet<LIKE_POST> LIKE_POSTS { get; set; }
        public virtual DbSet<MARKET> MARKETS { get; set; }
        public virtual DbSet<MARKET_PRODUCT> MARKET_PRODUCTS { get; set; }
        public virtual DbSet<MARKET_STORE> MARKET_STORES { get; set; }
        public virtual DbSet<ORDERS> ORDERS { get; set; }
        public virtual DbSet<POST> POSTS { get; set; }
        public virtual DbSet<PRODUCT> PRODUCTS { get; set; }
        public virtual DbSet<REPORT> REPORTS { get; set; }
        public virtual DbSet<RETURN> RETURNS { get; set; }
        public virtual DbSet<SET_UP_MARKETPLACE> SET_UP_MARKETPLACES { get; set; }
        public virtual DbSet<STORE> STORES { get; set; }
        public virtual DbSet<SUBMIT_AUTHENTICATION> SUBMIT_AUTHENTICATIONS { get; set; }
        public virtual DbSet<WALLET> WALLETS { get; set; }
        public virtual DbSet<POST_IMAGE> POST_IMAGES { get; set; }
        public virtual DbSet<SUB_CATEGORY> SUB_CATEGORYS { get; set; }
        public virtual DbSet<CATEGORY> CATEGORYS { get; set; }
        public virtual DbSet<PRODUCT_IMAGE> PRODUCT_IMAGES { get; set; }
        public virtual DbSet<PRODUCT_DETAIL> PRODUCT_DETAILS { get; set; }
        public virtual DbSet<STORE_BUSINESS_DIRECTION> STORE_BUSINESS_DIRECTIONS { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* 配置TPT表继承策略 */
            modelBuilder.Entity<ACCOUNT>(entity =>
            {
                entity.ToTable("ACCOUNT");

                entity.HasKey(e => e.ACCOUNT_ID);

                entity.Property(e => e.ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)");

                entity.Property(e => e.USER_NAME)
                       .HasMaxLength(30)
                       .HasColumnType("VARCHAR2(30)");

                entity.Property(e => e.EMAIL)
                       .HasMaxLength(50)
                       .HasColumnType("VARCHAR2(50)");

                entity.Property(e => e.PASSWORD)
                       .HasMaxLength(100)
                       .HasColumnType("VARCHAR2(100)")
                       .IsRequired();
                entity.Property(e => e.DESCRIBTION)
                       .HasMaxLength(400)
                       .HasColumnType("VARCHAR2(400)");
                entity.Property(e => e.PHOTO)
                    .HasColumnType("BLOB");

/*                                // 配置 ACCOUNT 和 WALLET 之间的一对一关系
                entity.HasOne(a => a.WALLET) // 从 ACCOUNT 出发
                      .WithOne(w => w.ACCOUNT) // 设置 WALLET 的导航属性
                      .HasForeignKey<WALLET>(w => w.ACCOUNT_ID) // 设置外键
                      .HasConstraintName("WALLET_FK"); // 外键约束名*/

            });

            modelBuilder.Entity<ADMINISTRATOR>(entity =>
            {
                entity.ToTable("ADMINISTRATOR");

                entity.Property(e => e.PERMISSION_LEVEL)
                       .HasColumnType("NUMBER(2)");

                entity.HasOne<ACCOUNT>()
                       .WithOne()
                       .HasForeignKey<ADMINISTRATOR>(e => e.ACCOUNT_ID)
                       .HasConstraintName("ADMINISTRATOR_FK");
            });

            modelBuilder.Entity<BUYER>(entity =>
            {
                entity.ToTable("BUYER");

                entity.Property(e => e.GENDER)
                      .HasMaxLength(10)
                      .HasColumnType("VARCHAR2(10)");

                entity.Property(e => e.AGE)
                      .HasColumnType("NUMBER(3)");

                entity.Property(e => e.TOTAL_CREDITS)
                      .HasColumnType("NUMBER(10)")
                      .IsRequired();

                entity.Property(e => e.ADDRESS)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)");
                      //.IsRequired();

                entity.HasOne<ACCOUNT>()
                      .WithOne()
                      .HasForeignKey<BUYER>(e => e.ACCOUNT_ID)
                      .HasConstraintName("BUYER_FK");

                //实验实验//////////////////////////////////////////////
                entity.HasMany(b => b.POSTS)
                    .WithOne(p => p.BUYER)
                    .HasForeignKey(p => p.ACCOUNT_ID)
                     .HasConstraintName("POST_FK");

                entity.HasMany(b => b.COMMENT_POSTS)
                        .WithOne(p => p.BUYER)
                        .HasForeignKey(p => p.BUYER_ACCOUNT_ID);

                entity.HasMany(b => b.COMMENT_COMMENTS)
                     .WithOne(p => p.BUYER)
                   .HasForeignKey(p => p.ACCOUNT_ID);

                entity.HasMany(b => b.COMPLAIN_POSTS)
                        .WithOne(p => p.BUYER)
                     .HasForeignKey(p => p.BUYER_ACCOUNT_ID);

                entity.HasMany(b => b.COMMENT_COMMENTS)
                     .WithOne(p => p.BUYER)
                    .HasForeignKey(p => p.ACCOUNT_ID);


            });

            modelBuilder.Entity<STORE>(entity =>
            {
                entity.ToTable("STORE");

                entity.Property(e => e.ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.STORE_NAME)
                      .HasMaxLength(20)
                      .HasColumnType("VARCHAR2(20)");

                entity.Property(e => e.STORE_SCORE)
                      .HasColumnType("NUMBER(2,1)")
                      .IsRequired();

                entity.Property(e => e.CERTIFICATION)
                      .HasColumnType("NUMBER(1)")
                      .HasConversion<bool>();

                entity.Property(e => e.ADDRESS)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)");

                entity.HasOne<ACCOUNT>()
                      .WithOne()
                      .HasForeignKey<STORE>(e => e.ACCOUNT_ID)
                      .HasConstraintName("A_A_FK1");

                entity.HasMany(b => b.STORE_BUSINESS_DIRECTIONS)
                    .WithOne(p => p.STORE)
                     .HasForeignKey(p => p.STORE_ID);//新增加
            });

            modelBuilder.Entity<BUYER_PRODUCT_BOOKMARK>(entity =>
            {
                entity.ToTable("BUYER_PRODUCT_BOOKMARK");

                entity.HasKey(e => new { e.BUYER_ACCOUNT_ID, e.PRODUCT_ID })
                      .HasName("BUYER_PRODUCT_BOOKMARK_PK");

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.PRODUCT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.HasOne(b => b.BUYER)
                      .WithMany()
                      .HasForeignKey(b => b.BUYER_ACCOUNT_ID)
                      .HasConstraintName("B_B_FK4");

                entity.HasOne(p => p.PRODUCT)
                      .WithMany()
                      .HasForeignKey(p => p.PRODUCT_ID)
                      .HasConstraintName("P_P_FK3");
            });

            modelBuilder.Entity<BUYER_STORE_BOOKMARK>(entity =>
            {
                entity.ToTable("BUYER_STORE_BOOKMARK");

                entity.HasKey(e => new { e.BUYER_ACCOUNT_ID, e.STORE_ACCOUNT_ID });

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.STORE_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.HasOne(b => b.BUYER)
                      .WithMany()
                      .HasForeignKey(b => b.BUYER_ACCOUNT_ID)
                      .HasConstraintName("B_B_FK");

                entity.HasOne(s => s.STORE)
                      .WithMany()
                      .HasForeignKey(e => e.STORE_ACCOUNT_ID)
                      .HasConstraintName("S_S_FK2");
            });

            modelBuilder.Entity<COMMENT_POST>(entity =>
            {
                entity.ToTable("COMMENT_POST");

                entity.HasKey(e => new { e.COMMENT_ID });

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();
                entity.Property(e => e.COMMENT_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.POST_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.EVALUATION_TIME)
                      .HasColumnType("TIMESTAMP(6)")
                      .IsRequired();

                entity.Property(e => e.EVALUATION_CONTENT)
                      .HasMaxLength(200)
                      .HasColumnType("VARCHAR2(200)");
                entity.Property(p => p.NUMBER_OF_SUBCOMMENTS)
                    .HasColumnType("NUMBER(10)")
                    .IsRequired();
                entity.Property(e => e.IS_READ)
                    .HasColumnType("NUMBER");
                /*                entity.HasOne(b => b.BUYER)
                                      .WithMany()
                                      .HasForeignKey(b => b.BUYER_ACCOUNT_ID)
                                      .HasConstraintName("B_B_FK1");*/
                entity.HasMany(b => b.COMMENT_COMMENTS)
                  .WithOne(p => p.COMMENT_POST)
               .HasForeignKey(p => p.COMMENT_ID);

/*                entity.HasOne(p => p.POST)
                      .WithMany()
                      .HasForeignKey(p => p.POST_ID)
                      .HasConstraintName("P_P_FK1");*/
            });

            modelBuilder.Entity<COMMENT_COMMENT>(entity =>
            {
                entity.ToTable("COMMENT_COMMENT");

                entity.HasKey(e => new { e.COMMENT_ID });

                entity.Property(e => e.ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();
                entity.Property(e => e.COMMENT_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.COMMENTED_COMMENT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.COMMENT_TIME)
                      .HasColumnType("TIMESTAMP(6)")
                      .IsRequired();

                entity.Property(e => e.COMMENT_CONTENT)
                      .HasMaxLength(200)
                      .HasColumnType("VARCHAR2(200)");

                entity.Property(e => e.IS_READ)
                       .HasColumnType("NUMBER")
                       .HasDefaultValueSql("0");

                /*                entity.HasOne(b => b.BUYER)
                                      .WithMany()
                                      .HasForeignKey(b => b.ACCOUNT_ID)
                                      .HasConstraintName("BUYER_ID_FK");*/

                /*                entity.HasOne(p => p.COMMENT_POST)
                                      .WithMany()
                                      .HasForeignKey(p => p.COMMENTED_COMMENT_ID)
                                      .HasConstraintName("COMMENT_POST_FK");*/
            });

            modelBuilder.Entity<COMPLAIN_POST>(entity =>
            {
                entity.ToTable("COMPLAIN_POST");

                entity.HasKey(e => new { e.BUYER_ACCOUNT_ID, e.POST_ID, e.REPORT_ID });

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.POST_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.REPORT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

/*                entity.HasOne(b => b.BUYER)
                      .WithMany()
                      .HasForeignKey(b => b.BUYER_ACCOUNT_ID)
                      .HasConstraintName("B_B_FK3");*/

                entity.HasOne(p => p.POST)
                      .WithMany()
                      .HasForeignKey(p => p.POST_ID)
                      .HasConstraintName("P_P_FK_3");

                entity.HasOne(r => r.REPORT)
                      .WithMany()
                      .HasForeignKey(r => r.REPORT_ID)
                      .HasConstraintName("R_R_FK");
            });

            modelBuilder.Entity<COMPLAIN_COMMENT>(entity =>
            {
                entity.ToTable("COMPLAIN_COMMENT");

                entity.HasKey(e => new { e.ACCOUNT_ID, e.COMMENT_ID, e.REPORT_ID });

                entity.Property(e => e.ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.COMMENT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.REPORT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

/*                entity.HasOne(b => b.BUYER)
                      .WithMany()
                      .HasForeignKey(b => b.ACCOUNT_ID)
                      .HasConstraintName("ACCOUNT_ID_FK");*/

                entity.HasOne(p => p.COMMENT_POST)
                      .WithMany()
                      .HasForeignKey(p => p.COMMENT_ID)
                      .HasConstraintName("COMMENT_ID_FK");

                entity.HasOne(r => r.REPORT)
                      .WithMany()
                      .HasForeignKey(r => r.REPORT_ID)
                      .HasConstraintName("REPORT_ID_FK");
            });

            modelBuilder.Entity<LIKE_POST>(entity =>
            {
                entity.ToTable("LIKE_POST");

                entity.HasKey(e => new { e.BUYER_ACCOUNT_ID, e.POST_ID });

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.POST_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.HasOne(b => b.BUYER)
                      .WithMany()
                      .HasForeignKey(b => b.BUYER_ACCOUNT_ID)
                      .HasConstraintName("B_B_FK2");

                entity.HasOne(p => p.POST)
                      .WithMany()
                      .HasForeignKey(p => p.POST_ID)
                      .HasConstraintName("P_P_FK2");
            });

            modelBuilder.Entity<MARKET>(entity =>
            {
                entity.ToTable("MARKET");

                entity.HasKey(e => e.MARKET_ID);

                entity.Property(e => e.MARKET_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.THEME)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(50)");

                entity.Property(e => e.START_TIME)
                      .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.END_TIME)
                      .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.DETAIL)
                      .HasMaxLength(500)
                      .HasColumnType("VARCHAR2(500)");
                entity.Property(e => e.POSTERIMG)
                    .HasColumnType("BLOB")
                    .IsRequired();
                entity.Property(e => e.IMAGE_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();
            });

            modelBuilder.Entity<MARKET_PRODUCT>(entity =>
            {
                entity.ToTable("MARKET_PRODUCT");

                entity.HasKey(e => new { e.MARKET_ID, e.PRODUCT_ID });

                entity.Property(e => e.MARKET_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.PRODUCT_ID)
                      .HasMaxLength(100)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.DISCOUNT_PRICE)
                      .HasColumnType("NUMBER(7,2)");

                entity.HasOne(m => m.MARKET)
                      .WithMany()
                      .HasForeignKey(e => e.MARKET_ID)
                      .HasConstraintName("M_M_FK");

                entity.HasOne(p => p.PRODUCT)
                      .WithMany()
                      .HasForeignKey(p => p.PRODUCT_ID)
                      .HasConstraintName("P_P_FK");
            });

            modelBuilder.Entity<PRODUCT>(entity =>
            {
                entity.HasKey(e => e.PRODUCT_ID);

                entity.ToTable("PRODUCT");

                entity.Property(e => e.PRODUCT_ID)
                    .HasMaxLength(100)
                    .HasColumnType("VARCHAR2(100)")
                    .IsRequired();

                entity.Property(e => e.PRODUCT_NAME)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.PRODUCT_PRICE)
                    .HasColumnType("NUMBER(7,2)")
                    .IsRequired();

                entity.Property(e => e.SALE_OR_NOT)
                    .HasColumnType("NUMBER(1)")
                    .HasConversion<bool>()
                    .IsRequired();

                entity.Property(e => e.TAG)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.SUB_TAG)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.STORE_TAG)
                    .HasMaxLength(50);

                entity.Property(e => e.DESCRIBTION)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasOne(s => s.STORE)
                    .WithMany()
                    .HasForeignKey(s => s.ACCOUNT_ID)
                    .HasConstraintName("PRODUCT_FK");

/*                entity.HasOne(e => e.SUB_CATEGORY)
                    .WithMany()
                    .HasForeignKey(e => e.TAG)
                    .HasConstraintName("PRODUCT_SUBCATEGORY_FK");*/

                //实验：
                entity.HasMany(b => b.PRODUCT_IMAGES)
                      .WithOne(p => p.PRODUCT)
                      .HasForeignKey(p => p.PRODUCT_ID);

                entity.HasMany(b => b.PRODUCT_DETAILS)
                      .WithOne(p => p.PRODUCT)
                      .HasForeignKey(p => p.PRODUCT_ID);
                /*                entity.HasOne(b => b.BUYER)
                                    .WithMany()
                                    .HasForeignKey(b => b.ACCOUNT_ID)
                                    .HasConstraintName("POST_FK")
                                    .IsRequired();*/
                //这个代码一加就有问题
            });

            modelBuilder.Entity<WALLET>(entity =>
            {
                entity.HasKey(e => e.ACCOUNT_ID);

                entity.ToTable("WALLET");

                entity.Property(e => e.ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.BALANCE)
                    .HasColumnType("NUMBER(10,2)")
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.HasOne(a => a.ACCOUNT)
                    .WithOne()
                    .HasForeignKey<WALLET>(a => a.ACCOUNT_ID)
                    .HasConstraintName("WALLET_FK");
            });

            modelBuilder.Entity<RETURN>(entity =>
            {
                entity.HasKey(e => e.ORDER_ID);

                entity.ToTable("RETURN");

                entity.Property(e => e.ORDER_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.RETURN_TIME)
                    .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.RETURN_REASON)
                    .HasMaxLength(100);

                entity.Property(e => e.RETURN_STATUS)
                    .HasMaxLength(100);

                entity.HasOne(o => o.ORDERS)
                    .WithOne()
                    .HasForeignKey<RETURN>(o => o.ORDER_ID)
                    .HasConstraintName("RETURN_FK");
            });

            modelBuilder.Entity<SET_UP_MARKETPLACE>(entity =>
            {
                entity.HasKey(a => new { a.MARKET_ID, a.ADMINISTRATOR_ACCOUNT_ID });

                entity.ToTable("SET_UP_MARKETPLACE");

                entity.Property(e => e.MARKET_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.ADMINISTRATOR_ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasOne(m => m.MARKET)
                    .WithOne()
                    .HasForeignKey<SET_UP_MARKETPLACE>(m => m.MARKET_ID)
                    .HasConstraintName("M_M_FK5");

                entity.HasOne(a => a.ADMINISTRATOR)
                    .WithOne()
                    .HasForeignKey<SET_UP_MARKETPLACE>(a => a.ADMINISTRATOR_ACCOUNT_ID)
                    .HasConstraintName("A_A_FK2");
            });

            modelBuilder.Entity<POST>(entity =>
            {
                entity.HasKey(e => e.POST_ID);

                entity.ToTable("POST");

                entity.Property(e => e.POST_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.RELEASE_TIME)
                    .HasColumnType("TIMESTAMP(6)")
                    .IsRequired();

                entity.Property(e => e.POST_CONTENT)
                    .HasMaxLength(800)
                    .IsRequired();

                entity.Property(p => p.NUMBER_OF_LIKES)
                    .HasColumnType("NUMBER(10)")
                    .IsRequired();

                entity.Property(p => p.NUMBER_OF_COMMENTS)
                    .HasColumnType("NUMBER(10)")
                    .IsRequired();


                entity.Property(p => p.ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.POST_TITLE)
                         .HasMaxLength(100)
                         .IsRequired();

                //实验：
                entity.HasMany(b => b.POST_IMAGES)
                      .WithOne(p => p.POST)
                      .HasForeignKey(p => p.POST_ID);
                entity.HasMany(b => b.COMMENT_POSTS)
                  .WithOne(p => p.POST)
                .HasForeignKey(p => p.POST_ID);
                /*                entity.HasOne(b => b.BUYER)
                                    .WithMany()
                                    .HasForeignKey(b => b.ACCOUNT_ID)
                                    .HasConstraintName("POST_FK")
                                    .IsRequired();*/
                //这个代码一加就有问题


            });

            modelBuilder.Entity<ORDERS>(entity =>
            {
                entity.HasKey(e => e.ORDER_ID);

                entity.ToTable("ORDERS");

                entity.Property(e => e.ORDER_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.PRODUCT_ID)
                    .HasMaxLength(100);

                entity.Property(e => e.TOTAL_PAY)
                    .HasColumnType("NUMBER(7, 2)");

                entity.Property(e => e.ACTUAL_PAY)
                    .HasColumnType("NUMBER(7, 2)");

                entity.Property(e => e.ORDER_STATUS)
                    .HasMaxLength(40);

                entity.Property(e => e.CREATE_TIME)
                    .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.RECIEVING_TIME)
                    .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.DELIVERY_NUMBER)
                    .HasMaxLength(100);

                entity.Property(e => e.SCORE)
                    .HasColumnType("NUMBER(2, 1)")
                    .HasDefaultValue(new decimal(0.0));

                entity.Property(e => e.REMARK)
                    .HasMaxLength(200);

                entity.Property(e => e.BONUS_CREDITS)
                    .HasColumnType("NUMBER(4)");

                entity.Property(e => e.RETURN_OR_NOT)
                    .HasColumnType("NUMBER(1)")
                    .HasConversion<bool>();

                entity.Property(e => e.BUYER_ACCOUNT_ID)
                    .HasMaxLength(100);

                entity.Property(e => e.STORE_ACCOUNT_ID)
                    .HasMaxLength(100);

                entity.HasOne(p => p.PRODUCT)
                    .WithOne()
                    .HasForeignKey<ORDERS>(p => p.PRODUCT_ID)
                    .HasConstraintName("O_P_FK");

                entity.HasOne(b => b.BUYER)
                    .WithOne()
                    .HasForeignKey<ORDERS>(b => b.BUYER_ACCOUNT_ID)
                    .HasConstraintName("O_B_FK");

                entity.HasOne(s => s.STORE)
                    .WithMany()
                    .HasForeignKey(s => s.STORE_ACCOUNT_ID)
                    .HasConstraintName("S_S_FK");
            });

            modelBuilder.Entity<REPORT>(entity =>
            {
                entity.HasKey(e => e.REPORT_ID);

                entity.ToTable("REPORT");

                entity.Property(e => e.REPORT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.REPORT_TIME)
                    .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.REPORT_REASON)
                    .HasMaxLength(200);

                entity.Property(e => e.AUDIT_TIME)
                    .HasColumnType("TIMESTAMP(6)");

                entity.Property(e => e.AUDIT_RESULTS)
                    .HasMaxLength(10);

                entity.Property(e => e.ADMINISTRATOR_ACCOUNT_ID)
                    .HasMaxLength(100);

                entity.HasOne(a => a.ADMINISTRATOR)
                    .WithMany()
                    .HasForeignKey(a => a.ADMINISTRATOR_ACCOUNT_ID)
                    .HasConstraintName("REPORT_FK");
            });

            modelBuilder.Entity<MARKET_STORE>(entity =>
            {
                entity.HasKey(e => new { e.MARKET_ID, e.STORE_ACCOUNT_ID });

                entity.ToTable("MARKET_STORE");

                entity.Property(e => e.MARKET_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.STORE_ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.IN_OR_NOT)
                    .HasColumnType("NUMBER(1)")
                    .HasConversion<bool>()
                    .IsRequired();

                entity.HasOne(m => m.MARKET)
                    .WithMany()
                    .HasForeignKey(m => m.MARKET_ID)
                    .HasConstraintName("M_M_FK1");

                entity.HasOne(s => s.STORE)
                    .WithMany()
                    .HasForeignKey(s => s.STORE_ACCOUNT_ID)
                    .HasConstraintName("S_S_FK1");
            });

            modelBuilder.Entity<SUBMIT_AUTHENTICATION>(entity =>
            {
                entity.HasKey(e => new { e.ADMINISTRATOR_ACCOUNT_ID, e.STORE_ACCOUNT_ID });

                entity.ToTable("SUBMIT_AUTHENTICATION");

                entity.Property(e => e.STORE_ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.ADMINISTRATOR_ACCOUNT_ID)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.AUTHENTICATION)
                    .HasMaxLength(200);

                entity.Property(e => e.STATUS)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(e => e.PHOTO)
                    .HasColumnType("BLOB");

                entity.HasOne(s => s.STORE)
                    .WithOne()
                    .HasForeignKey<SUBMIT_AUTHENTICATION>(s => s.STORE_ACCOUNT_ID)
                    .HasConstraintName("S_S_FK3");

                entity.HasOne(a => a.ADMINISTRATOR)
                    .WithMany()
                    .HasForeignKey(a => a.ADMINISTRATOR_ACCOUNT_ID)
                    .HasConstraintName("A_A_FK");
            });

            modelBuilder.Entity<POST_IMAGE>(entity =>
            {
                entity.ToTable("POST_IMAGE");

                entity.HasKey(e => new { e.IMAGE_ID } );

                entity.Property(e => e.POST_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE)
                      .HasColumnType("BLOB")
                      .IsRequired();

                /*                entity.HasOne(p => p.POST)
                                      .WithMany()
                                      .HasForeignKey(p => p.POST_ID)
                                      .HasConstraintName("POST_ID_FK");*/
            });

            modelBuilder.Entity<CATEGORY>(entity =>
            {
                entity.ToTable("CATEGORY");

                entity.HasKey(e => e.CATEGORY_NAME);

                entity.Property(e => e.CATEGORY_NAME)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(50)")
                      .IsRequired();

                entity.Property(e => e.CATEGORY_PIC)
                      .HasColumnType("BLOB");

                entity.Property(e => e.CATEGORY_DESCRIPTION)
                      .HasMaxLength(600)
                      .HasColumnType("VARCHAR2(600)");

                entity.HasMany(b => b.SUB_CATEGORYS)
                     .WithOne(p => p.CATEGORY)
                     .HasForeignKey(p => p.CATEGORY_NAME)
                     .HasConstraintName("SUB_CATEGORY_FK");

                entity.HasMany(b => b.PRODUCTS)
                     .WithOne(p => p.CATEGORY)
                     .HasForeignKey(p => p.TAG)
                     .HasConstraintName("PRODUCT_BIG_CATEGORY_FK");
            });

            modelBuilder.Entity<SUB_CATEGORY>(entity =>
            {
                entity.ToTable("SUB_CATEGORY");

                entity.HasKey(e => e.SUBCATEGORY_ID);

                entity.Property(e => e.SUBCATEGORY_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(50)")
                      .IsRequired();

                entity.Property(e => e.CATEGORY_NAME)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(50)")
                      .IsRequired();

                entity.Property(e => e.SUBCATEGORY_NAME)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(50)")
                      .IsRequired();

/*                entity.HasOne(e => e.CATEGORY)
                      .WithMany()
                      .HasForeignKey(e => e.CATEGORY_NAME)
                      .HasConstraintName("SUB_CATEGORY_FK");*/

                entity.HasMany(b => b.PRODUCTS)
                      .WithOne(p => p.SUB_CATEGORY)
                      .HasForeignKey(p => p.SUB_TAG)
                      .HasConstraintName("PRODUCT_SUBCATEGORY_FK"); 
            });

            modelBuilder.Entity<PRODUCT_IMAGE>(entity =>
            {
                entity.ToTable("PRODUCT_IMAGE");

                entity.HasKey(e => new { e.IMAGE_ID });

                entity.Property(e => e.PRODUCT_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE)
                      .HasColumnType("BLOB")
                      .IsRequired();
            });

            modelBuilder.Entity<PRODUCT_DETAIL>(entity =>
            {
                entity.ToTable("PRODUCT_DETAIL");

                entity.HasKey(e => new { e.IMAGE_ID });

                entity.Property(e => e.PRODUCT_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE_ID)
                      .HasMaxLength(50)
                      .HasColumnType("VARCHAR2(100)")
                      .IsRequired();

                entity.Property(e => e.IMAGE)
                      .HasColumnType("BLOB")
                      .IsRequired();

                entity.Property(e => e.DESCRIPTION)
                      .HasMaxLength(500)
                      .HasColumnType("VARCHAR2(500)");
            });

            modelBuilder.Entity<STORE_BUSINESS_DIRECTION>(entity =>
            {
                entity.HasKey(e => new { e.STORE_ID,e.BUSINESS_TAG});

                entity.ToTable("STORE_BUSINESS_DIRECTION");

                entity.Property(e => e.STORE_ID)
                    .HasMaxLength(100)
                    .HasColumnType("VARCHAR2(100)")
                    .IsRequired();
                entity.Property(e => e.BUSINESS_TAG)
                    .HasMaxLength(50)
                    .HasColumnType("VARCHAR2(50)")
                    .IsRequired();
                entity.Property(e => e.LINK_COUNT)
                    .HasColumnType("NUMBER(10)");

            });
        }
    }
}
