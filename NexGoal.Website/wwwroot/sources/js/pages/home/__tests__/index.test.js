import React from "react";
import { shallow, render } from "enzyme";
import renderer from 'react-test-renderer';
import mockAxios from "axios";
import Mypage from '../index';

jest.mock('axios');
jest.mock('moment', () => () => ({ format: () => '2018–05–11T12:00:00+00:00' }));
jest.mock('../../../resourceHelper');

describe('Render correctly', () => {
    const component = renderer.create(<Mypage />);
    expect(component.toJSON()).toMatchSnapshot();
});