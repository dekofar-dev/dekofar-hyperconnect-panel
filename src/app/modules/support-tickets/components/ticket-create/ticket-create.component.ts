import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SupportTicketService } from '../../services/support-ticket.service';
import { Router } from '@angular/router';
import { ModalComponent, ModalConfig } from 'src/app/_metronic/partials';
import { SupportCategory } from '../../models/support-category.enum';
import { SupportTicketCreateDto } from '../../models/support-ticket.model';
import { AuthService } from 'src/app/modules/auth';

@Component({
  selector: 'app-ticket-create',
  templateUrl: './ticket-create.component.html',
  styleUrls: ['./ticket-create.component.scss']
})
export class TicketCreateComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  ticketCode = '';
  attachedFiles: File[] = [];
  includeOrder = false;
  errorDetails = '';
  isShopifyReference = false;
  orderSummary: any = null;

  @ViewChild('errorModal') errorModal!: ModalComponent;

  modalConfig: ModalConfig = {
    modalTitle: 'Hata Oluştu',
    dismissButtonLabel: 'Kapat',
    closeButtonLabel: 'Detay Göster'
  };

  categories: { id: number; label: string }[] = [];

  priorityOptions = [
    { id: 1, label: '🔴 Yüksek' },
    { id: 2, label: '🟡 Orta' },
    { id: 3, label: '🟢 Düşük' }
  ];

  constructor(
    private fb: FormBuilder,
    private ticketService: SupportTicketService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFromLocalStorage();
    this.isShopifyReference = !!this.orderSummary;
    this.categories = this.getCategoryOptions();
    this.initForm();
    this.handleCategoryChange();
  }

  initForm(): void {
    this.form = this.fb.group({
      subject: ['', Validators.required],
      category: [null, Validators.required],
      priority: [null, Validators.required],
      description: ['', Validators.required],
      tags: [''],
      customerName: [
        this.isShopifyReference ? this.orderSummary?.customerName : '',
        this.isShopifyReference ? [] : Validators.required
      ],
      customerPhone: [
        this.isShopifyReference ? this.orderSummary?.customerPhone : '',
        this.isShopifyReference ? [] : Validators.required
      ],
      customerEmail: [
        this.isShopifyReference ? this.orderSummary?.customerEmail : '',
        Validators.email
      ],
      shopifyOrderId: [this.isShopifyReference ? this.orderSummary?.orderNumber : null],
      dueDate: [null]
    });
  }

  handleCategoryChange(): void {
    this.form.get('category')?.valueChanges.subscribe((categoryId) => {
      const customerNameControl = this.form.get('customerName');
      const customerPhoneControl = this.form.get('customerPhone');

      // Örneğin sadece İade kategorisinde bu alanlar zorunlu olacaksa:
      const iadeKategoriId = this.categories.find(c => c.label === 'İade')?.id;

      if (categoryId === iadeKategoriId && !this.isShopifyReference) {
        customerNameControl?.setValidators(Validators.required);
        customerPhoneControl?.setValidators(Validators.required);
      } else {
        customerNameControl?.clearValidators();
        customerPhoneControl?.clearValidators();
      }

      customerNameControl?.updateValueAndValidity();
      customerPhoneControl?.updateValueAndValidity();
    });
  }

  getCategoryOptions(): { id: number; label: string }[] {
    return Object.keys(SupportCategory)
      .filter(key => !isNaN(Number(SupportCategory[key as keyof typeof SupportCategory])))
      .map(key => ({
        id: SupportCategory[key as keyof typeof SupportCategory],
        label: key
      }));
  }

  loadFromLocalStorage(): void {
    const stored = localStorage.getItem('pendingTicket');
    if (!stored) return;

    try {
      const ticketData = JSON.parse(stored);
      this.orderSummary = ticketData.orderSummary || null;
      this.includeOrder = !!ticketData.shopifyOrderId;
      this.ticketCode = ticketData.subject || '';
      localStorage.removeItem('pendingTicket');
    } catch (err) {
      console.error('❌ Ticket verisi yüklenemedi:', err);
    }
  }

  onFileSelected(event: any): void {
    const files: FileList = event.target.files;
    for (let i = 0; i < files.length; i++) {
      this.attachedFiles.push(files[i]);
    }
  }

  submit(): void {
    if (this.form.invalid) {
      console.warn('⚠️ Form geçersiz:', this.form.value);
      return;
    }

    this.loading = true;
    const ticketData: SupportTicketCreateDto = this.form.value;

    if (!this.isShopifyReference) {
      ticketData.shopifyOrderId = undefined;
    }

    this.ticketService.create(ticketData, this.attachedFiles).subscribe({
      next: () => {
        this.loading = false;
        alert('✅ Destek talebi başarıyla oluşturuldu.');
        this.router.navigate(['/support-tickets']);
      },
      error: (err) => {
        this.loading = false;
        this.errorDetails = JSON.stringify(err.error || err.message || err, null, 2);
        this.openErrorModal();
      }
    });
  }

  async openErrorModal() {
    await this.errorModal.open();
  }

  showFullError() {
    console.error('📄 Hata Detayı:', this.errorDetails);
    this.errorModal.close();
  }
}
