namespace RagSystem.Domain.Entities;

public abstract class Entity
{
  /// <summary>
  /// Identificador único do banco (UUID)
  /// </summary>
  public Guid Id { get; protected set; }

  /// <summary>
  /// Data e hora da criação do registro
  /// </summary>
  public DateTime CreatedAt { get; protected set; }

  /// <summary>
  /// Data e hora da última atualização do registro
  /// </summary>
  public DateTime? UpdatedAt { get; protected set; }

}
