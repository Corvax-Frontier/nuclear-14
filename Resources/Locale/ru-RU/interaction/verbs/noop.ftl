interaction-LookAt-name = Смотреть
interaction-LookAt-description = Уставься в пустоту и смотри, как она смотрит обратно.
interaction-LookAt-success-self-popup = Вы смотрите на {THE($target)}.
interaction-LookAt-success-target-popup = Вы чувствуете, как {THE($user)} смотрит на вас...
interaction-LookAt-success-others-popup = {THE($user)} смотрит на {THE($target)}.

interaction-Hug-name = Обнять
interaction-Hug-description = Всего одно объятие в день способно избавить от психологических проблем, которые вам кажутся непреодолимыми.
interaction-Hug-success-self-popup = Вы обнимаете {THE($target)}.
interaction-Hug-success-target-popup = {THE($user)} обнимает вас.
interaction-Hug-success-others-popup = {THE($user)} обнимает {THE($target)}.

interaction-Pet-name = Погладить
interaction-Pet-description = Погладьте своего коллегу, чтобы снять его стресс.
interaction-Pet-success-self-popup = Вы гладите {THE($target)} по {POSS-ADJ($target)} голове.
interaction-Pet-success-target-popup = {THE($user)} гладит вас по {POSS-ADJ($target)} голове.
interaction-Pet-success-others-popup = {THE($user)} гладит {THE($target)}.

interaction-PetAnimal-name = {interaction-Pet-name}
interaction-PetAnimal-description = Погладьте животное.
interaction-PetAnimal-success-self-popup = {interaction-Pet-success-self-popup}
interaction-PetAnimal-success-target-popup = {interaction-Pet-success-target-popup}
interaction-PetAnimal-success-others-popup = {interaction-Pet-success-others-popup}

interaction-KnockOn-name = Постучать
interaction-KnockOn-description = Постучите по цели, чтобы привлечь внимание.
interaction-KnockOn-success-self-popup = Вы стучите по {THE($target)}.
interaction-KnockOn-success-target-popup = {THE($user)} стучит по вам.
interaction-KnockOn-success-others-popup = {THE($user)} стучит по {THE($target)}.

interaction-Rattle-name = Потрясти
interaction-Rattle-success-self-popup = Вы трясете {THE($target)}.
interaction-Rattle-success-target-popup = {THE($user)} трясет вас.
interaction-Rattle-success-others-popup = {THE($user)} трясет {THE($target)}.

# The below includes conditionals for if the user is holding an item
interaction-WaveAt-name = Помахать
interaction-WaveAt-description = Помашите цели. Если у вас есть предмет, вы будете махать им.
interaction-WaveAt-success-self-popup = Вы машете {$hasUsed ->
    [false] {THE($target)}.
    *[true] своим {$used} {THE($target)}.
}
interaction-WaveAt-success-target-popup = {THE($user)} машет {$hasUsed ->
    [false] вам.
    *[true] {POSS-PRONOUN($user)} {$used} вам.
}
interaction-WaveAt-success-others-popup = {THE($user)} машет {$hasUsed ->
    [false] {THE($target)}.
    *[true] {POSS-PRONOUN($user)} {$used} {THE($target)}.
}
