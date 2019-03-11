import React from 'react';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';
import Announcement from '../Announcement';

describe('Announcement Tests', () => {
    test('Render', () => {
        const component = renderer.create(
            <Announcement announcements={[]} />,
        );

        expect(component.toJSON()).toMatchSnapshot();
    });

    test('Render all announcements', () => {
        const announcements = ['123', '456'];
        const component = shallow(
            <Announcement announcements={announcements} />,
        );

        expect(component.find('li')).toHaveLength(announcements.length);
    });
});