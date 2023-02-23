function ret = dft321()
    len = 300;
    x = 0:1:len;
    freq = [10,17,25];
    step = 3*2*pi/len;
    ampl = zeros(3,len);
    for j=1:3
        for i =1:len
            ampl(j,i) = sin(freq(j)*step*x(i));
        end
    end
    dft3 = ddft321(ampl(1,:), ampl(2,:), ampl(3,:));
    ret = ifft(dft3);

    function ddft321 = ddft321(Ax, Ay, Az)
        L = length(Ax);
        n = 2^nextpow2(L);
        AX = fft(Ax,n);
        AY = fft(Ay,n);
        AZ = fft(Az,n);
        ddft321 = zeros(1,n);
        for k=1:n
            ddft321(k) = sqrt(AX(k)*AX(k) + AY(k)*AY(k) + AZ(k)*AZ(k));
        end
    end
end