export function getPriorityInfo(priority: number | undefined): { label: string; icon: string; color: string } {
  switch (priority) {
    case 2:
      return { label: 'Yüksek', icon: '🔴', color: 'danger' };
    case 1:
      return { label: 'Orta', icon: '🟡', color: 'warning' };
    case 0:
      return { label: 'Düşük', icon: '🟢', color: 'success' };
    default:
      return { label: 'Bilinmiyor', icon: '⚪', color: 'secondary' };
  }
}

export function getStatusInfo(status: number | undefined): { label: string; color: string; icon: string } {
  switch (status) {
    case 0:
      return { label: 'Açık', color: 'primary', icon: '📬' };
    case 1:
      return { label: 'Yanıtlandı', color: 'info', icon: '📤' };
    case 2:
      return { label: 'Tamamlandı', color: 'success', icon: '✅' };
    case 3:
      return { label: 'İptal Edildi', color: 'danger', icon: '❌' };
    default:
      return { label: 'Bilinmiyor', color: 'secondary', icon: '❓' };
  }
}
