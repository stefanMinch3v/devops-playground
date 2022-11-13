namespace TaskTronic.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;
    using System;

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        private const string SERIALIZED_DATA = "serializedData";

        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property<string>(SERIALIZED_DATA)
                .IsRequired()
                .HasField(SERIALIZED_DATA);

            builder
                .Property(m => m.Type)
                .IsRequired()
                .HasConversion(
                    t => t.AssemblyQualifiedName,
                    t => Type.GetType(t));
        }
    }
}
