import React, { useEffect, useState } from "react";
import { Animated } from "react-native";

//TODO: fix out the animated problem.
function FadeInImage(props) {
    const [opacity, setState] = useState(new Animated.Value(0),);
    useEffect(() => {
        Animated.timing(opacity, { toValue: 1, useNativeDriver: true, duration: 1000 }).start();
    }, [props.source]);

    return (
        <Animated.Image style={{ ...props.style, opacity: opacity }} source={props.source} />
    );
}

export default React.memo(FadeInImage);