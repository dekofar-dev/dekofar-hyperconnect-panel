import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-manual-order-create',
  templateUrl: './manual-order-create.component.html',
  styleUrl: './manual-order-create.component.scss'
})
export class ManualOrderCreateComponent implements OnInit {
  form: FormGroup;
  isSubmitting = false;
  products: any[] = []; // Shopify ürünleri
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private productService: ProductService,
    private router: Router
  ) {
    this.form = this.fb.group({
      customerName: ['', Validators.required],
      customerSurname: [''],
      phone: [''],
      email: [''],
      address: [''],
      city: [''],
      district: [''],
      paymentType: ['Havale'],
      orderNote: [''],
      discountName: [''],
      items: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getAllProducts().subscribe({
      next: (res) => this.products = res,
      error: (err) => {
        this.errorMessage = 'Ürünler yüklenemedi.';
        console.error(err);
      }
    });
  }

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  addItem(): void {
    this.items.push(this.fb.group({
      productId: ['', Validators.required],
      productName: [''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      price: [0, [Validators.required, Validators.min(0)]]
    }));
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  onProductSelect(index: number): void {
    const item = this.items.at(index);
    const productId = item.get('productId')?.value;
    const product = this.products.find(p => p.id === productId);
    if (product) {
      item.patchValue({
        productName: product.title,
        price: product.variants?.[0]?.price || 0
      });
    }
  }

  submit(): void {
    if (this.form.invalid) return;

    this.isSubmitting = true;
    this.orderService.createManualOrder(this.form.value).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/orders']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = 'Sipariş oluşturulamadı.';
        console.error(err);
      }
    });
  }
}
