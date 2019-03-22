import React from 'react';
import { shallow } from 'enzyme';
import { Button, Form, Modal } from 'react-bootstrap';

import ConfirmationModal from '../ConfirmationModal';

describe('<ConfirmationModal />', () => {
    test('renders correctly', () => {
        const wrapper = shallow(
            <ConfirmationModal />
        );

        expect(wrapper.find(Modal)).toHaveLength(1);
        expect(wrapper.find(Form)).toHaveLength(1);
        expect(wrapper.find(Button)).toHaveLength(2);
    });

    //it('simulates click events', () => {
    //    const onButtonClick = jest.fn();
    //    const wrapper = mount(
    //        <ConfirmationModal showModel />
    //    );

    //    wrapper.find('btn-cancel').simulate('click');
    //    expect(onButtonClick).toHaveBeenCalled();
    //});
});