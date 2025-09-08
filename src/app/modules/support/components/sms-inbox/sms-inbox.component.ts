import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { NetgsmSmsService, SmsMessageVm } from '../../services/netgsm-sms.service';

@Component({
  selector: 'app-sms-inbox',
  templateUrl: './sms-inbox.component.html',
  styleUrls: ['./sms-inbox.component.scss']
})
export class SmsInboxComponent implements OnInit, OnDestroy {
  loading = false;

  all: SmsMessageVm[] = [];
  rows: SmsMessageVm[] = [];
  selected?: SmsMessageVm;

  page = 1;
  pageSize = 20;
  total = 0;

  form: FormGroup;
  private subs = new Subscription();

  constructor(
    private fb: FormBuilder,
    private netgsm: NetgsmSmsService
  ) {
    this.form = this.fb.group({
      from: [this.toInputDate(this.shiftDays(-7)), Validators.required],
      to:   [this.toInputDate(new Date()), Validators.required],
      search: ['']
    });
  }

  ngOnInit(): void {
    // canlı arama
    this.subs.add(
      this.form.get('search')!.valueChanges.subscribe(() => this.applyFilters(true))
    );

    // sayfa açıldığında otomatik son 7 gün getir
    this.refreshLastDays();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  /** Manuel tarih aralığına göre yükle (Getir butonu) */
  load(): void {
    if (this.form.invalid) return;
    this.loading = true;

    const from = new Date(this.form.value.from + 'T00:00:00');
    const to   = new Date(this.form.value.to + 'T23:59:59');

    this.subs.add(
      this.netgsm.getSmsInboxByRange(from, to).subscribe({
        next: data => {
          this.all = data;
          this.applyFilters(true);
        },
        error: _ => {
          this.all = [];
          this.applyFilters(true);
        },
        complete: () => (this.loading = false)
      })
    );
  }

  /** Varsayılan: Son 7 günü getir */
  private refreshLastDays(): void {
    this.loading = true;
    this.subs.add(
      this.netgsm.getSmsInboxLastDays(7).subscribe({
        next: data => {
          this.all = data;
          this.applyFilters(true);
        },
        error: _ => {
          this.all = [];
          this.applyFilters(true);
        },
        complete: () => (this.loading = false)
      })
    );
  }

  applyFilters(resetPage = false): void {
    const term = this.form.value.search || '';
    const filtered = this.netgsm.filterLocal(this.all, term);
    this.total = filtered.length;

    if (resetPage) this.page = 1;
    this.rows = this.netgsm.paginateLocal(filtered, this.page, this.pageSize);

    if (this.selected) {
      const stillExists = filtered.find(
        x =>
          x.fromNumber === this.selected!.fromNumber &&
          x.receivedAt.getTime() === this.selected!.receivedAt.getTime()
      );
      if (!stillExists) this.selected = undefined;
    }
  }

  onPage(delta: number): void {
    const maxPage = Math.max(1, Math.ceil(this.total / this.pageSize));
    this.page = Math.min(maxPage, Math.max(1, this.page + delta));
    this.applyFilters();
  }

  selectRow(r: SmsMessageVm): void {
    this.selected = r;
  }

  // ---------- yardımcılar ----------
  private shiftDays(d: number): Date {
    const x = new Date();
    x.setDate(x.getDate() + d);
    return x;
  }

  private toInputDate(date: Date): string {
    const p = (n: number) => (n < 10 ? '0' + n : '' + n);
    return `${date.getFullYear()}-${p(date.getMonth() + 1)}-${p(date.getDate())}`;
  }
}
